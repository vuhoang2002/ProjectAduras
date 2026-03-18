using System;
using System.Collections;
using UnityEngine;

public class PlayerGuardState : PlayerBaseState
{
    float guardLayerWeight = 0f;
    private float perfectBlockTime = 0.5f;
    private float timer;
    public PlayerGuardState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void _OnEnter()
    {
        _SMch.curentState = "Guard";
        _CbCtrl.SetGuard(true);
        _SMch.EnterCombatMode = true;
        //_SMch.Animator.SetLayerWeight(1, 1);
        _SMch.InputReader.ReleaseSecondaryHand += OnSAttackReleased;
        _SMch.InputReader.isSprinting = false;
    }
    public override void _OnExit()
    {
        _SMch.Animator.SetLayerWeight(1, 0);
        _CbCtrl.SetGuard(false);
        _SMch.InputReader.ReleaseSecondaryHand -= OnSAttackReleased;
    }



    public override void _OnUpdate(float tick)
    {
        if (perfectBlockTime > 0)
        {
            perfectBlockTime -= Time.deltaTime;
        }
        Vector3 movement = _SMch.CalculateMovementByLook();
        // _SM.CharacterController.Move(movement * _SM.playerFreeLookSpeed * tick);
        AdurasMove(movement * _SMch.getMoveSpeed(), tick);
        if (_SMch.InputReader.VectorMovement == Vector2.zero)
        {
            _SMch.Animator.SetFloat(AdurasAnimHash.MoveAmountHash, 0f, 0.1f, tick);
            _SMch.Animator.SetFloat(AdurasAnimHash.HorizontalHash, 0f, 0.1f, tick);
            return;
        }
        _SMch.RotateTowardCameraForward(tick);
        Vector2 valueSpeed = _SMch.getValueAnimSpeed(_SMch.InputReader.VectorMovement);
        _SMch.Animator.SetFloat(AdurasAnimHash.MoveAmountHash, valueSpeed.y, 0.1f, tick);
        _SMch.Animator.SetFloat(AdurasAnimHash.HorizontalHash, valueSpeed.x, 0.1f, tick);

    }
    private void OnSAttackReleased()
    {
        _SMch._SwitchState(new PlayerFreeLookState(this._SMch));
    }

}
