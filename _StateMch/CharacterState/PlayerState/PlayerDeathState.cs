using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void _OnEnter()
    {
        _SMch.Ragdoll.ToggleRagdoll(true);
        _SMch.curentState = "Death";
    }

    public override void _OnExit()
    {
    }

    public override void _OnUpdate(float tick)
    {
    }
}
