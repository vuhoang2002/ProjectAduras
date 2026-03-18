using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using static cbValue;

public class PlayerAttackState : PlayerBaseState
{
    const float AnimationDampTime = 0.1f;


    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    bool doCombo = false;
    bool isComboing = false; // it make a second attack not interrupt first attack
    private Coroutine atkCoroutine;
    private bool chargingComplete = false;
    private bool releaseChargeSoon = false;
    public override void _OnEnter()
    {
        _SMch.curentState = "Attack";

        _SMch.InputReader.PressPrimaryHand += OnPAttack;
        _SMch.InputReader.ReleasePrimaryHand += OnPAttackReleased;
        _SMch.InputReader.PressSecondaryHand += OnSAttack;
        _SMch.InputReader.ReleaseSecondaryHand += OnSAttackReleased;
        _SMch.InputReader.DodgeEvent += HandleDodge;
        _SMch.InputReader.JumpEvent += HandleJump;
        _CbCtrl.IsStanding = false;
        //  _SM.ChangeWeight(1, 0, 1);
        //when enter attack state check which hand is active to perform attack
        if (_SMch.InputReader.IsPActive)
        {
            OnPAttack();//accpet input
        }
        else if (_SMch.InputReader.IsSActive)
        {
            OnSAttack();
        }
    }
    public override void _OnExit()
    {
        _SMch.InputReader.PressPrimaryHand -= OnPAttack;
        _SMch.InputReader.ReleasePrimaryHand -= OnPAttackReleased;
        _SMch.InputReader.PressSecondaryHand -= OnSAttack;
        _SMch.InputReader.ReleaseSecondaryHand -= OnSAttackReleased;
        _SMch.InputReader.DodgeEvent -= HandleDodge;
        _SMch.InputReader.JumpEvent -= HandleJump;
        _SMch.isAttacking = false;
        _SMch._RootMotion = false;
        _CbCtrl.ResetAttackPhase();
        _CbCtrl.ResetComboCount();
        _CbCtrl.IsStanding = true;
        _SMch.InputReader.resetPress();
        if (atkCoroutine != null)
        {
            _SMch.StopCoroutine(atkCoroutine);
        }
        _CbCtrl.ActiveWeaponHixBox(false);
        //todo :có cái này sẽ bị giật khi kết thúc đòn đánh bị giât có thể do chuyển animator controller liên tục, cần kiểm tra lại sau
        //_SMch.RevertToBaseAnimatorController(); 



    }
    public override void _OnUpdate(float tick)
    {
        if (_SMch.isAttacking)
        {
            // if (_CbCtrl.IsPhase(AttackPhase.None) || _CbCtrl.IsPhase(AttackPhase.Over))
            //     _SMch.FindBestTarget();

            _SMch.AssistRotateToTarget(tick);
        }

        AdurasMove(tick);
    }
    private void OnPAttack()
    {
        HandleClickAttack(CbArm.Primary);
    }
    private void OnPAttackReleased()
    {
        HandleReleaseAttack(CbArm.Primary);
    }
    private void OnSAttack()
    {
        HandleClickAttack(CbArm.Secondary);
    }
    private void OnSAttackReleased()
    {
        HandleReleaseAttack(CbArm.Secondary);
    }
    public void HandleClickAttack(CbArm cbHand)
    {


        _CbCtrl.currentSkillData = _CbCtrl.cbGetter._getCurrentSkill(cbHand, getCbSkill());
        //  _SM.Printf("Current Skill Data:" + curentSkillData.name);
        if (_CbCtrl.currentSkillData.attackProperty == AttackProperty.Melee)
            _SMch.FindBestTarget();

        if (_CbCtrl.currentSkillData == null)
        {
            _CbCtrl.currentSkillData = _CbCtrl.cbGetter._getDefaultSkill(cbHand);
        }

        if (_CbCtrl.IsPhase(AttackPhase.Impact) || _CbCtrl.IsPhase(AttackPhase.CoolDown) || _CbCtrl.IsPhase(AttackPhase.Over))
        {
            if (_CbCtrl.cbGetter.GetAttackType(_CbCtrl.currentSkillData) == AttackType.Normal)
            {
                doCombo = true;
                //  _CbCtrl.currentPhaseString = _CbCtrl.CurrentPhase.ToString() + doCombo;

            }
        }
        if (_SMch.isAttacking || isComboing)
        {
            return;
        }
        if (_CbCtrl.IsPhase(AttackPhase.None) || _CbCtrl.IsPhase(AttackPhase.Over))
        {

            if (_CbCtrl.cbGetter.GetAttackType(_CbCtrl.currentSkillData) == AttackType.Hold)
            {
                //handle hold attack here
            }
            else if (_CbCtrl.cbGetter.GetAttackType(_CbCtrl.currentSkillData) == AttackType.Normal)
            {
                atkCoroutine = _SMch.StartCoroutine(AttackNormalCoroutine(_CbCtrl.getComboCount(), _CbCtrl.currentSkillData));

            }
            else if (_CbCtrl.cbGetter.GetAttackType(_CbCtrl.currentSkillData) == AttackType.Charging || _CbCtrl.cbGetter.GetAttackType(_CbCtrl.currentSkillData) == AttackType.Charge_Normal)
            {
                atkCoroutine = _SMch.StartCoroutine(AttackChargingCoroutine_Start(_CbCtrl.getComboCount(), _CbCtrl.currentSkillData));
            }
            else if (_CbCtrl.cbGetter.GetAttackType(_CbCtrl.currentSkillData) == AttackType.DefShield)
            {
                // atkCoroutine = _SMch.StartCoroutine(DefShieldCoroutine(_CbCtrl.currentSkillData));
                _SMch._SwitchState(new PlayerGuardState(this._SMch));
            }

        }
    }
    public void HandleReleaseAttack(CbArm cbHand)
    {

        if (_CbCtrl.cbGetter.GetAttackType(_CbCtrl.currentSkillData) == AttackType.Charging || _CbCtrl.cbGetter.GetAttackType(_CbCtrl.currentSkillData) == AttackType.Charge_Normal)
        {
            if (_CbCtrl.IsPhase(AttackPhase.Charging) && chargingComplete == true)
            {
                _SMch.StopCoroutine(atkCoroutine);
                atkCoroutine = _SMch.StartCoroutine(AttackChargingCoroutine_Release(_CbCtrl.getComboCount(), _CbCtrl.currentSkillData));
                chargingComplete = false;
            }
            else if (_CbCtrl.IsPhase(AttackPhase.Charging) && chargingComplete == false)
            {
                releaseChargeSoon = true;
            }

            else
            {
                _SMch._SwitchState(new PlayerFreeLookState(_SMch));
            }
        }
    }

    private CbSkill getCbSkill()
    {
        return _SMch.InputReader.CbSkill;
    }
    #region Normal
    private IEnumerator AttackNormalCoroutine(int comboCount, SkillData skDt)
    {
        int maxCombo = skDt.BaseAtkDt.Count - 1;
        bool outOfCountCombo = comboCount >= maxCombo;
        comboCount = Mathf.Clamp(comboCount, 0, maxCombo);
        if (!AttackCostCalculation(skDt.BaseAtkDt[comboCount]))
            _SMch._SwitchState(new PlayerFreeLookState(_SMch));
        _CbCtrl.SetAttackPhase(AttackPhase.WindUp, skDt, true);
        //_SM.ChangeWeight(1, 0, 1);
        _SMch._RootMotion = true;
        _SMch.isAttacking = true;
        isComboing = true;
        if (skDt.aoe != null)
            _SMch.Animator.runtimeAnimatorController = skDt.aoe;
        int impactCount = 0; //the number hit of one animation
        int skillImpactCount = _CbCtrl.cbGetter.getImpactCount(skDt, comboCount, true);
        bool completeAllHitBox = false;
        bool finalCombo = false;
        if (!outOfCountCombo)
            _SMch.Animator.CrossFade(_CbCtrl.cbGetter.getAnim_Release(skDt, comboCount, 0, true), AnimationDampTime);
        else
            _SMch.Animator.CrossFade(AdurasAnimHash._FinalComboHash, AnimationDampTime);
        yield return null;
        var aState = _SMch.Animator.GetNextAnimatorStateInfo(0);
        float timer = 0f;
        while (timer < aState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / aState.length;//[0->1]
            if (_CbCtrl.IsPhase(AttackPhase.WindUp))
            {
                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_ImpactTime(skDt, comboCount, 0, true))
                {
                    _CbCtrl.SetAttackPhase(AttackPhase.Impact, skDt, true, impactCount);
                    if (_CbCtrl.cbGetter.isAddCombo(skDt, comboCount, true))
                    {
                        _CbCtrl.addComboCount();
                    }
                    else if (_CbCtrl.cbGetter.isResetCombo(skDt, comboCount, true))
                    {
                        finalCombo = true;
                        _CbCtrl.ResetComboCount();
                    }
                }
            }
            else if (_CbCtrl.IsPhase(AttackPhase.Impact))
            {
                //_SMch.Printf("Impact count:" + impactCount + "skillImpactCount:" + skillImpactCount + "completeAllHitBox:" + completeAllHitBox + "comboCount:" + comboCount + "impactCount:" + impactCount);
                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_CoolDownTime(skDt, comboCount, impactCount + 1, true))
                {
                    // _SM.Printf("normalizedTime:" + normalizedTime + "sang cooldown");
                    impactCount += 2;
                    if (impactCount >= skillImpactCount)
                    {
                        completeAllHitBox = true;
                        _CbCtrl.SetAttackPhase(AttackPhase.CoolDown, skDt, true, impactCount);
                    }
                    _CbCtrl.SetAttackPhase(AttackPhase.CoolDown, skDt, true, impactCount);
                }
            }
            else if (_CbCtrl.IsPhase(AttackPhase.CoolDown))
            {
                if (completeAllHitBox)
                {
                    _SMch.isAttacking = false;
                    _SMch._RootMotion = false;
                    _CbCtrl.IsStanding = true;
                    _CbCtrl.SetAttackPhase(AttackPhase.Over, skDt, true);
                }
                else if (!completeAllHitBox && normalizedTime >= _CbCtrl.cbGetter.getAnim_ImpactTime(skDt, comboCount, impactCount, true))
                {
                    //_SMch.Printf("Chuyển lại về impact" + impactCount + normalizedTime);
                    _CbCtrl.SetAttackPhase(AttackPhase.Impact, skDt, true, impactCount);
                }
            }
            else if (_CbCtrl.IsPhase(AttackPhase.Over))
            {
                _SMch.isAttacking = false;

                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_OverTime(skDt, comboCount, true))
                {
                    _CbCtrl.SetAttackPhase(AttackPhase.None, skDt, true);
                    _CbCtrl.ResetComboCount();
                }
                if (doCombo && !finalCombo)
                {
                    doCombo = false;

                    atkCoroutine = _SMch.StartCoroutine(AttackNormalCoroutine(_CbCtrl.getComboCount(), _CbCtrl.currentSkillData));
                    yield break;
                }

            }
            else if (_CbCtrl.IsPhase(AttackPhase.None))
            {
                if (normalizedTime >= 1f)
                {
                    isComboing = false;
                    _SMch._SwitchState(new PlayerFreeLookState(_SMch));

                }
            }

            //
            yield return null;
        }
    }
    #endregion
    #region  Charging
    private IEnumerator AttackChargingCoroutine_Start(int comboCount, SkillData skDt)
    {
        _CbCtrl.SetAttackPhase(AttackPhase.Charging, skDt, true);
        int chargingLevel = 0;
        //_SM.ChangeWeight(1, 0, 1);
        _SMch._RootMotion = true;
        _SMch.isAttacking = true;
        isComboing = true;
        if (skDt.aoe != null)
            _SMch.Animator.runtimeAnimatorController = skDt.aoe;
        _SMch.Animator.CrossFade(_CbCtrl.cbGetter.getAnim_Charging(skDt, 0), AnimationDampTime);
        if (skDt.effectSkillCharging != null)
            _CbCtrl.CastEffectWhenCharge(skDt.effectSkillCharging);
        yield return null;
        // var aState = _SMch.Animator.GetNextAnimatorClipInfo(0);
        float timer = 0f;
        while (timer <= skDt.chargeTime[skDt.chargeTime.Count - 1])
        {
            timer += Time.deltaTime;
            yield return null;
            if (chargingLevel >= skDt.chargeTime.Count)
            {
                break;
            }
            if (releaseChargeSoon && chargingLevel >= 1)
            {
                chargingLevel = 1;// khi nhả quá sớm thì tự động charign đòn lv1
                releaseChargeSoon = false;
                break;
            }
            if (chargingLevel >= 1)
            {
                chargingComplete = true;
            }
            if (timer > skDt.chargeTime[chargingLevel])
            {
                chargingLevel++;
                // _SMch.Printf("Từ lúc tăng " + chargingLevel + "time:" + _SMch.InputReader.pHoldTime + " timer2: " + timer);
            }
        }
        chargingComplete = false;
        atkCoroutine = _SMch.StartCoroutine(AttackChargingCoroutine_Release(_CbCtrl.getComboCount(), _CbCtrl.currentSkillData));
        yield break;

    }




    private IEnumerator AttackChargingCoroutine_Release(int comboCount, SkillData skDt)
    {
        //        _SMch.Printf("Combo count:" + comboCount);
        int impactCount = 0;
        bool finalCombo = false;
        int skillImpactCount = _CbCtrl.cbGetter.getImpactCount(skDt, 0, false);
        bool completeAllHitBox = false;

        _SMch.Animator.CrossFade(_CbCtrl.cbGetter.getAnim_Release(skDt, 0, 0, false), AnimationDampTime);//no combocount for charging attack
        yield return null;
        var aState = _SMch.Animator.GetNextAnimatorStateInfo(0);
        float timer = 0f;
        if (skDt.effectSkillCharging != null)
        {
            _CbCtrl.DestroyEffect(skDt.effectSkillCharging.context);
        }
        while (timer < aState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / aState.length;//[0->1]
            if (_CbCtrl.IsPhase(AttackPhase.Charging))
            {
                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_ImpactTime(skDt, comboCount, 0, false))
                {
                    _CbCtrl.SetAttackPhase(AttackPhase.Impact, skDt, false, impactCount);
                    if (_CbCtrl.cbGetter.isAddCombo(skDt, comboCount, false))
                    {
                        _CbCtrl.addComboCount();
                    }
                    else if (_CbCtrl.cbGetter.isResetCombo(skDt, comboCount, false))
                    {
                        finalCombo = true;
                        _CbCtrl.ResetComboCount();
                    }
                }
            }
            else if (_CbCtrl.IsPhase(AttackPhase.Impact))
            {
                //_SMch.Printf("Impact count:" + impactCount + "skillImpactCount:" + skillImpactCount + "completeAllHitBox:" + completeAllHitBox + "comboCount:" + comboCount + "impactCount:" + impactCount);
                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_CoolDownTime(skDt, comboCount, impactCount + 1, false))
                {
                    // _SM.Printf("normalizedTime:" + normalizedTime + "sang cooldown");
                    impactCount += 2;
                    if (impactCount >= skillImpactCount)
                    {
                        completeAllHitBox = true;
                        _CbCtrl.SetAttackPhase(AttackPhase.CoolDown, skDt, false, impactCount);
                    }
                    _CbCtrl.SetAttackPhase(AttackPhase.CoolDown, skDt, false, impactCount);
                }
            }
            else if (_CbCtrl.IsPhase(AttackPhase.CoolDown))
            {
                if (completeAllHitBox)
                {
                    _SMch.isAttacking = false;
                    _SMch._RootMotion = false;
                    _CbCtrl.IsStanding = true;
                    _CbCtrl.SetAttackPhase(AttackPhase.Over, skDt, false);
                }
                else if (!completeAllHitBox && normalizedTime >= _CbCtrl.cbGetter.getAnim_ImpactTime(skDt, comboCount, impactCount, false))
                {
                    //_SMch.Printf("Chuyển lại về impact" + impactCount + normalizedTime);
                    _CbCtrl.SetAttackPhase(AttackPhase.Impact, skDt, false, impactCount);
                }
            }
            else if (_CbCtrl.IsPhase(AttackPhase.Over))
            {
                _SMch.isAttacking = false;

                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_OverTime(skDt, comboCount, false))
                {
                    _CbCtrl.SetAttackPhase(AttackPhase.None, skDt, false);
                    _CbCtrl.ResetComboCount();
                }
                if (doCombo && !finalCombo)
                {
                    doCombo = false;

                    atkCoroutine = _SMch.StartCoroutine(AttackNormalCoroutine(_CbCtrl.getComboCount(), _CbCtrl.currentSkillData));
                    yield break;
                }

            }
            else if (_CbCtrl.IsPhase(AttackPhase.None))
            {
                if (normalizedTime >= 1f)
                {
                    isComboing = false;
                    _SMch._SwitchState(new PlayerFreeLookState(_SMch));

                }
            }
            //
            yield return null;
        }
        _CbCtrl.SetAttackPhase(AttackPhase.None, skDt, false);
        _SMch._SwitchState(new PlayerFreeLookState(_SMch));

    }

    #endregion
    #region Hold 

    #endregion
    public void HandleDodge()
    {
        if (_CbCtrl.IsPhase(AttackPhase.None) || _CbCtrl.IsPhase(AttackPhase.Over))
            _SMch._SwitchState(new PlayerDodgeState(_SMch, _SMch.InputReader.VectorMovement));
    }
    public void HandleJump()
    {
        if (_CbCtrl.IsPhase(AttackPhase.Over) || _CbCtrl.IsPhase(AttackPhase.None))
        {
            _SMch._SwitchState(new PlayerJumpState(_SMch));
        }
    }
    #region DefShield
    IEnumerator DefShieldCoroutine(SkillData skdt)
    {
        _SMch._SwitchState(new PlayerGuardState(this._SMch));
        yield return null;

    }
    #endregion
}


