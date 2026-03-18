using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void _OnEnter()
    {
        _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.FreeLookBlendTreeHash, 0.5f);
        _SMch.eCbBehavius = eCombatState.Idle;

    }

    public override void _OnExit()
    {

    }

    public override void _OnUpdate(float tick)
    {
        SearchingPlayerByFOV();
        AdurasMove(tick);
    }

}
