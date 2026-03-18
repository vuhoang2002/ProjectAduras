using System;
using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
    // variable StateMachine was called at parent
    const float AnimatorDampTime = 0.05f;
    public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void _OnEnter()
    {
        if (_SMch.EnterCombatMode)
        {
            _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.CombatBlendTreeHash, 0.2f);
        }
        else
        {
            _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.FreeLookBlendTreeHash, 0.2f);
        }
        //_SMch.InputReader.SprintEvent += HandleSprint;
        _SMch.InputReader.JumpEvent += HandleJump;
        _SMch.InputReader.DodgeEvent += HandleDodge;
        _SMch.curentState = "FreeLook";
    }



    public override void _OnExit()
    {
        //_SMch.InputReader.SprintEvent -= HandleSprint;
        _SMch.InputReader.JumpEvent -= HandleJump;
        _SMch.InputReader.DodgeEvent -= HandleDodge;

    }



    public override void _OnUpdate(float tick)
    {
        if (CheckPressingAttack())
        {
            _SMch._SwitchState(new PlayerAttackState(_SMch));
            return;
        }
        if (!_SMch.isGrounded)
        {
            _SMch._SwitchState(new PlayerLoopFallingState(_SMch));
            return;
        }
        Vector3 movement = _SMch.CalculateMovementByLook();
        // _SM.CharacterController.Move(movement * _SM.playerFreeLookSpeed * tick);
        AdurasMove(movement * _SMch.getMoveSpeed(), tick);
        if (_SMch.InputReader.VectorMovement == Vector2.zero)
        {
            _SMch.Animator.SetFloat(AdurasAnimHash.MoveAmountHash, 0f, AnimatorDampTime, tick);
            _SMch.Animator.SetFloat(AdurasAnimHash.HorizontalHash, 0f, AnimatorDampTime, tick);

            return;
        }
        if (!_SMch.EnterCombatMode)
        {
            FaceMoveDirection(movement, tick);
            _SMch.Animator.SetFloat(AdurasAnimHash.MoveAmountHash, _SMch.getAnimBySpeed(), AnimatorDampTime, tick);
        }
        else
        {
            _SMch.RotateTowardCameraForward(tick);
            Vector2 valueSpeed = _SMch.getValueAnimSpeed(_SMch.InputReader.VectorMovement);
            _SMch.Animator.SetFloat(AdurasAnimHash.MoveAmountHash, valueSpeed.y, AnimatorDampTime, tick);
            _SMch.Animator.SetFloat(AdurasAnimHash.HorizontalHash, valueSpeed.x, AnimatorDampTime, tick);

        }
    }


    private void FaceMoveDirection(Vector3 movementDir, float deltaTime)
    {
        _SMch.transform.rotation = Quaternion.Lerp(
            _SMch.transform.rotation,
            Quaternion.LookRotation(movementDir.normalized),
                _SMch.rotationSmoothValue * deltaTime
        );
    }
    private void HandleJump()
    {
        _SMch._SwitchState(new PlayerJumpState(_SMch));
    }
    private void HandleDodge()
    {
        _SMch._SwitchState(new PlayerDodgeState(_SMch, _SMch.InputReader.VectorMovement));
    }
}
