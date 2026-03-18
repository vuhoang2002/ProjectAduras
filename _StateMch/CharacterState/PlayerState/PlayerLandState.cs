using UnityEngine;

public class PlayerLandState : PlayerBaseState
{
    const float AnimatorDampTime = 0.1f;
    public PlayerLandState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void _OnEnter()
    {
        _SMch.curentState = "Land";

        _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.LandHash, 0.1f);
    }

    public override void _OnExit()
    {
    }

    public override void _OnUpdate(float tick)
    {
        if (_SMch.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            _SMch._SwitchState(new PlayerFreeLookState(_SMch));
            return;
        }
    }

}
