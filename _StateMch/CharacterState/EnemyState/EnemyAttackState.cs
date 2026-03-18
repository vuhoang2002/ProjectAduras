using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static cbValue;

public class EnemyAttackState : EnemyBaseState
{//incule move to player 1f then attack
    private float attackDistance = 3f;
    private float AnimationDampTime = 0.1f;
    bool doCombo;
    bool isComboing = false; // it make a second attack not interrupt first attack
    bool isAttacked = false;
    float percentCombo = 50f;
    bool isBreak = false;
    private Coroutine atkCoroutine;
    private NPCCombatBehavius NPCCombatBehavius;
    private bool releaseChargeSoon = false;
    private bool chargingComplete = false;
    private float chargeRandomTime = 0f;
    public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void _OnEnter()
    {
        _SMch.eCbBehavius = eCombatState.Attack;
        NPCCombatBehavius = _SMch.GetComponent<NPCCombatBehavius>();
        Vector2 distance = new Vector2(NPCCombatBehavius.AICombatBehaviour.closeDistance, NPCCombatBehavius.AICombatBehaviour.rangerDistance);
        attackDistance = Random.Range(distance.x, distance.y);
        _SMch.Agent.stoppingDistance = attackDistance;
        doCombo = CanCombo();
        ChoseSkillAttack();
    }

    public override void _OnExit()
    {
        _SMch.Agent.ResetPath();
        if (atkCoroutine != null)
        {
            _SMch.StopCoroutine(atkCoroutine);
        }
        _CbCtrl.ResetComboCount();
        _SMch._RootMotion = false;
        _SMch.isAttacking = false;
        // _SMch.Agent.enabled = true;
        _CbCtrl.ActiveWeaponHixBox(false);
        NPCCombatBehavius.ResetScore();
    }

    public override void _OnUpdate(float tick)
    {

        if (_SMch.isAttacking)
        {
            _SMch.FindBestTarget();
            _SMch.AssistRotateToTarget(tick);
            return;
        }
        if (_SMch.Target == null)
        {
            _SMch._SwitchState(new EnemyIdleState(this._SMch));
            return;
        }
        if (Vector3.Distance(_SMch.transform.position, _SMch.Target.transform.position) > attackDistance + 0.3f)
        {
            if (isAttacked) return;
            _SMch.Agent.SetDestination(_SMch.Target.transform.position);
        }
        else
        {
            if (isAttacked) return;
            _SMch.Agent.ResetPath();
            // _SMch.Printf("Enemy Attack State Attack now");
            if (_CbCtrl.currentSkillData.attackType == AttackType.Normal)
                atkCoroutine = _SMch.StartCoroutine(AttackNormalCoroutine(_CbCtrl.getComboCount(), _CbCtrl.currentSkillData));
            else if (_CbCtrl.currentSkillData.attackType == AttackType.Charging)
                atkCoroutine = _SMch.StartCoroutine(AttackChargingCoroutine_Start(_CbCtrl.getComboCount(), _CbCtrl.currentSkillData));
            isAttacked = true;
        }
    }

    private void ChoseSkillAttack()
    {
        // _CbCtrl.currentSkillData = _CbCtrl.cbGetter._getCurrentSkill(CbArm.Primary, CbSkill.None);
        //_CbCtrl.currentSkillData = _CbCtrl.combatHandData.E_Skill_PHand;
        _CbCtrl.currentSkillData = NPCCombatBehavius.CaculateScore(attackDistance);
        if (_CbCtrl.currentSkillData.attackProperty == AttackProperty.Melee)
        {
            attackDistance = 0.7f;
            _SMch.Agent.stoppingDistance = 0.7f;
        }

    }
    #region  Normal
    private IEnumerator AttackNormalCoroutine(int comboCount, SkillData skDt)
    {
        //_SMch.Printf("EnemyAttack COmbo:" + comboCount);
        _CbCtrl.SetAttackPhase(AttackPhase.WindUp, skDt, true);
        //_SM.ChangeWeight(1, 0, 1);
        _SMch._RootMotion = true;
        _SMch.isAttacking = true;
        isComboing = true;
        bool outOfCountCombo = false;
        if (comboCount >= skDt.BaseAtkDt.Count - 1)
        {
            comboCount = skDt.BaseAtkDt.Count - 1;
            outOfCountCombo = true;
        }//this may take bug same animation name
        _SMch.Animator.runtimeAnimatorController = skDt.aoe;
        int impactCount = 0; //the number hit of one animation
        int skillImpactCount = skDt.BaseAtkDt[comboCount].ImpactTime.Count;
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
            //  _SMch.Printf("Enemy Attack State Normalized Time:" + normalizedTime + "Impact count:" + impactCount);
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
                //  _SMch.Printf("Impact count:" + impactCount + "skillImpactCount:" + skillImpactCount + "completeAllHitBox:" + completeAllHitBox + "_comboCount:" + comboCount);
                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_CoolDownTime(skDt, comboCount, impactCount + 1, true))
                {
                    impactCount += 2;
                    if (impactCount >= skillImpactCount)
                    {
                        completeAllHitBox = true;
                    }
                    _CbCtrl.SetAttackPhase(AttackPhase.CoolDown, skDt, true, impactCount);
                }
            }
            else if (_CbCtrl.IsPhase(AttackPhase.CoolDown))
            {
                if (completeAllHitBox)
                {
                    _SMch._RootMotion = false;
                    _CbCtrl.SetAttackPhase(AttackPhase.Over, skDt, true);
                }
                else if (!completeAllHitBox && normalizedTime >= _CbCtrl.cbGetter.getAnim_ImpactTime(skDt, comboCount, impactCount, true))
                {
                    // _SMch.Printf("Chuyển lại về impact" + impactCount + "time%" + normalizedTime);
                    _CbCtrl.SetAttackPhase(AttackPhase.Impact, skDt, true, impactCount);
                }
            }
            else if (_CbCtrl.IsPhase(AttackPhase.Over))
            {
                _SMch.isAttacking = false;

                if (doCombo && !finalCombo)
                {
                    doCombo = CanCombo();
                    //  _SMch.Printf("EndAttack:cb " + comboCount);
                    atkCoroutine = _SMch.StartCoroutine(AttackNormalCoroutine(_CbCtrl.getComboCount(), _CbCtrl.currentSkillData));
                    yield break;
                }
                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_OverTime(skDt, comboCount, true))
                {
                    _CbCtrl.SetAttackPhase(AttackPhase.None, skDt, true);
                    _CbCtrl.ResetComboCount();
                }

            }
            else if (_CbCtrl.IsPhase(AttackPhase.None))
            {
                if (normalizedTime >= 1f)
                {
                    isComboing = false;
                    _SMch._SwitchState(new EnemyRetreatAfterAttackState(_SMch));

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
        // _SMch.Agent.enabled = false;
        isComboing = true;
        if (skDt.aoe != null)
            _SMch.Animator.runtimeAnimatorController = skDt.aoe;
        _SMch.Animator.CrossFade(_CbCtrl.cbGetter.getAnim_Charging(skDt, 0), AnimationDampTime);
        if (skDt.effectSkillCharging != null)
            _CbCtrl.CastEffectWhenCharge(skDt.effectSkillCharging);
        yield return null;
        // var aState = _SMch.Animator.GetNextAnimatorClipInfo(0);
        float timer = 0f;
        float chargeRandomTime = Random.Range(skDt.chargeTime[0], skDt.chargeTime[skDt.chargeTime.Count - 1]);
        while (timer <= chargeRandomTime)
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
            _CbCtrl.DestroyEffect(skDt.effectSkillCharging.context);
        while (timer < aState.length)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / aState.length;//[0->1]
            if (_CbCtrl.IsPhase(AttackPhase.Charging))
            {
                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_ImpactTime(skDt, 0, 0, false))
                {
                    _CbCtrl.SetAttackPhase(AttackPhase.Impact, skDt, false, impactCount);
                    if (_CbCtrl.cbGetter.isAddCombo(skDt, 0, false))
                    {
                        _CbCtrl.addComboCount();
                    }
                    else if (_CbCtrl.cbGetter.isResetCombo(skDt, 0, false))
                    {
                        finalCombo = true;
                        _CbCtrl.ResetComboCount();
                    }
                }
            }
            else if (_CbCtrl.IsPhase(AttackPhase.Impact))
            {
                //_SMch.Printf("Impact count:" + impactCount + "skillImpactCount:" + skillImpactCount + "completeAllHitBox:" + completeAllHitBox + "comboCount:" + comboCount + "impactCount:" + impactCount);
                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_CoolDownTime(skDt, 0, impactCount + 1, false))
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
                //_SMch.isAttacking = false;

                if (normalizedTime >= _CbCtrl.cbGetter.getAnim_OverTime(skDt, 0, false))
                {
                    _CbCtrl.SetAttackPhase(AttackPhase.None, skDt, false);
                    _CbCtrl.ResetComboCount();
                }
                if (doCombo && !finalCombo)
                {
                    doCombo = false;
                    //atkCoroutine = _SMch.StartCoroutine(AttackNormalCoroutine(_CbCtrl.getComboCount(), _CbCtrl.currentSkillData));
                    //  yield break;
                }

            }
            else if (_CbCtrl.IsPhase(AttackPhase.None))
            {

                if (normalizedTime >= 1f)
                {
                    isComboing = false;
                    _SMch._RootMotion = false;
                    _SMch.isAttacking = false;
                    _SMch._SwitchState(new EnemyRetreatAfterAttackState(_SMch));

                }
            }
            //
            yield return null;
        }
        _CbCtrl.SetAttackPhase(AttackPhase.None, skDt, false);
        _SMch._SwitchState(new EnemyIdleState(_SMch));

    }

    #endregion
    private bool CanCombo()
    {
        doCombo = Random.Range(0, 100) < percentCombo ? true : false;
        return doCombo;
    }
}