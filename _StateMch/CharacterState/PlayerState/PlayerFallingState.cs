using UnityEngine;

public class PlayerFallingState : PlayerBaseState
{
    private Vector3 FallMomentum;
    public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void _OnEnter()
    {
        _SMch.curentState = "Falling";

        _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.FallingHash, 0.1f);
        FallMomentum = _SMch.CharacterController.velocity;
        FallMomentum.y = 0;
        _SMch.InputReader.JumpEvent += HandleDoubleJump;
    }

    public override void _OnExit()
    {
        _SMch.InputReader.JumpEvent -= HandleDoubleJump;

    }

    public override void _OnUpdate(float tick)
    {
        AdurasMove(FallMomentum, tick);
        if (_SMch.doubleJump == cbValue.DoubleJump.Action)
        {
            if (_SMch.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                _SMch._SwitchState(new PlayerDoubleJumpState(_SMch));
                _SMch.doubleJump = cbValue.DoubleJump.None;
                return;
            }
        }
        if (_SMch.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            _SMch._SwitchState(new PlayerLoopFallingState(_SMch));
            return;
        }

    }
    private void HandleDoubleJump()
    {
        if (_SMch.doubleJump == cbValue.DoubleJump.None)
        {
            _SMch.doubleJump = cbValue.DoubleJump.Action;
        }
    }

}
