using UnityEngine;

public class EnemyDeathState : EnemyBaseState
{
    public EnemyDeathState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void _OnEnter()
    {
        _SMch.Ragdoll.ToggleRagdoll(true);
        _SMch.eCbBehavius = eCombatState.Death;
    }

    public override void _OnExit()
    {
    }

    public override void _OnUpdate(float tick)
    {
    }
}
