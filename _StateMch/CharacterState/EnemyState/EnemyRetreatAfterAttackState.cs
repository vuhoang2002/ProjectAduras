using UnityEngine;

public class EnemyRetreatAfterAttackState : EnemyBaseState
{
    private float distanceToRetreat = 3f;//same as distanceToStand of EnemyCombatState
    public EnemyRetreatAfterAttackState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void _OnEnter()
    {
        _SMch.eCbBehavius = eCombatState.RetreatAfterAttack;
        _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.CombatBlendTreeHash, 0.5f);

    }

    public override void _OnExit()
    {

    }

    public override void _OnUpdate(float tick)
    {
        if (_SMch.Target == null)
        {
            _SMch._SwitchState(new EnemyIdleState(_SMch));
        }
        if (Vector3.Distance(_SMch.Target.transform.position, _SMch.transform.position) >= distanceToRetreat)
        {
            _SMch._SwitchState(new EnemyCombatState(_SMch));
            return;
        }
        //retreat from player
        Vector3 dirToPlayer = _SMch.transform.position - _SMch.Target.transform.position;
        _SMch.Agent.Move(dirToPlayer.normalized * _SMch.WalkSpeed * tick);
        //rotate to face player
        dirToPlayer.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(-dirToPlayer);
        _SMch.transform.rotation = Quaternion.Slerp(_SMch.transform.rotation, lookRotation, 300f * tick);
    }
}
