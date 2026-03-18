
using System;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private Vector3 JumpMomentum;
    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void _OnEnter()
    {
        _SMch.curentState = "Jump";
        _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.JumpHash, 0.1f);
        _SMch.ForceReceiver.Jump(_SMch.JumpForce);
        JumpMomentum = _SMch.CharacterController.velocity;
        JumpMomentum.y = 0;
        _SMch.InputReader.JumpEvent += HandleDoubleJump;

    }

    public override void _OnExit()
    {
        _SMch.InputReader.JumpEvent -= HandleDoubleJump;
    }



    public override void _OnUpdate(float tick)
    {
        AdurasMove(JumpMomentum, tick);
        if (_SMch.doubleJump == cbValue.DoubleJump.Action)
        {
            if (_SMch.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                _SMch._SwitchState(new PlayerDoubleJumpState(_SMch));
                _SMch.doubleJump = cbValue.DoubleJump.Done;
                return;
            }
        }

        if (_SMch.CharacterController.velocity.y < 0)
        {
            _SMch._SwitchState(new PlayerFallingState(_SMch));
            return;
        }
    }
    private void HandleDoubleJump()
    {
        _SMch.doubleJump = cbValue.DoubleJump.Action;
    }

}
