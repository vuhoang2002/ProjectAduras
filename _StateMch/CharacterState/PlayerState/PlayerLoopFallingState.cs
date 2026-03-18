using UnityEngine;

public class PlayerLoopFallingState : PlayerBaseState
{
    public PlayerLoopFallingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    Vector3 FallMomentum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void _OnEnter()
    {
        _SMch.curentState = "LoopFalling";

        _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.LoopFallHash, 0.1f);
        FallMomentum = _SMch.CharacterController.velocity;
        FallMomentum.y = 0;
    }

    public override void _OnExit()
    {

    }

    public override void _OnUpdate(float tick)
    {
        AdurasMove(FallMomentum, tick);
        if (_SMch.isGrounded)
        {
            _SMch._SwitchState(new PlayerLandState(_SMch));
            return;
        }
    }
}
