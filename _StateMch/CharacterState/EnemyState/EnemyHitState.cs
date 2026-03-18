
using System.Collections;
using UnityEngine;
using static cbValue;
public class EnemyHitState : EnemyBaseState
{
    private KnockOnEffect _knockOnEffect;
    private Vector3 force;
    private Vector3 impactPos;
    public EnemyHitState(EnemyStateMachine stateMachine, KnockOnEffect knockOnEffect, Vector3 force, Vector3 impactPos) : base(stateMachine)
    {
        this._knockOnEffect = knockOnEffect;
        this.force = force;
        this.impactPos = impactPos;
    }
    private Coroutine hitCoroutine;

    public override void _OnEnter()
    {
        _SMch.Animator.CrossFadeInFixedTime(GetAnimByKnockOnEffect(_knockOnEffect), 0.1f);
        hitCoroutine = _SMch.StartCoroutine(WaitAndSwitchState());
        _SMch.eCbBehavius = eCombatState.Hit;
        //_SMch.test(_SMch.transform.position, impactPos);
        Vector3 direction = (_SMch.transform.position - impactPos).normalized;
        // Vector3 finalForce = new Vector3(direction.x * force.x, direction.y, direction.z * force.z);
        Vector3 finalForce = direction * force.magnitude;
        // optional: giữ lại Y nếu muốn knock up
        if (_knockOnEffect == KnockOnEffect.KnockDown)
        {
            finalForce += Vector3.up * force.y;
        }
        _SMch.ForceReceiver.AddForce(finalForce);
        _SMch.Agent.enabled = false;
    }

    public override void _OnExit()
    {
        _SMch.Agent.enabled = true;
        if (hitCoroutine != null)
        {
            _SMch.StopCoroutine(hitCoroutine);
        }
        _CbCtrl.ResetAttackPhase();

    }

    public override void _OnUpdate(float tick)
    {
        AdurasMove(tick);
    }
    private IEnumerator WaitAndSwitchState()
    {
        yield return null;
        var aState = _SMch.Animator.GetNextAnimatorStateInfo(0);
        float timer = 0f;
        float normialTime;
        while (timer < aState.length)
        {
            normialTime = timer / aState.length;
            if (normialTime >= 0.9f)
            {
                _SMch.ForceReceiver.ResetForces();
            }
            timer += Time.deltaTime;
            yield return null;
        }
        if (_knockOnEffect == KnockOnEffect.KnockDown)
        {
            yield return new WaitForSeconds(2.5f);//todo: sửa thành chuyển sang animation đứng dậy rồi mới cho phép chuyển state, tránh việc nhân vật đứng dậy rồi mới chuyển sang idle state
        }
        _SMch._SwitchState(new EnemyIdleState(_SMch));
    }
    private int GetAnimByKnockOnEffect(KnockOnEffect knockOnEffect)
    {
        switch (knockOnEffect)
        {
            case KnockOnEffect.MiniStun:
                return GetMiniStunAnim();
            case KnockOnEffect.KnockBack:
                return AdurasAnimHash.HitKnockBackHash;
            case KnockOnEffect.KnockDown:
                return AdurasAnimHash.HitKnockDownHash;
            case KnockOnEffect.HeadDown:
                return AdurasAnimHash.HitHeadDownHash;
            default: return AdurasAnimHash.Hit01Hash;
        }
    }
    private int GetMiniStunAnim()
    {
        var stateInfo = _SMch.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == AdurasAnimHash.Hit01Hash)
            return AdurasAnimHash.Hit02Hash;

        if (stateInfo.shortNameHash == AdurasAnimHash.Hit02Hash)
            return AdurasAnimHash.Hit01Hash;

        return AdurasAnimHash.Hit01Hash;
    }

}
