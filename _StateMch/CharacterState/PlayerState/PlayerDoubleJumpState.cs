using UnityEngine;

public class PlayerDoubleJumpState : PlayerBaseState
{
    private Vector3 JumpMomentum;
    private float jumpForcePartTwo = 5f;
    public PlayerDoubleJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void _OnEnter()
    {
        _SMch.curentState = "DoubleJump";
        _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.DoubleJump, 0.1f);
        _SMch.ForceReceiver.Jump(jumpForcePartTwo);
        JumpMomentum = _SMch.CharacterController.velocity;
        JumpMomentum.y = 0;
        _SMch.doubleJump = cbValue.DoubleJump.None;

    }

    public override void _OnExit()
    {
    }

    public override void _OnUpdate(float tick)
    {
        AdurasMove(JumpMomentum, tick);


        if (_SMch.CharacterController.velocity.y < 0)
        {
            _SMch._SwitchState(new PlayerFallingState(_SMch));
            return;
        }
    }
}
