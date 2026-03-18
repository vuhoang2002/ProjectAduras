using UnityEngine;

public class PlayerDodgeState : PlayerBaseState
{
    private Vector2 dodgdeDir;
    private float dodgeSpeed = 4f;
    private float dogdeDuration = 0.5f;
    public PlayerDodgeState(PlayerStateMachine stateMachine, Vector2 dodgeDirection) : base(stateMachine)
    {
        this.dodgdeDir = dodgeDirection;
    }
    public override void _OnEnter()
    {
        _SMch.curentState = "Dodge";

        if (_SMch.EnterCombatMode)
        {

            _SMch.Animator.SetFloat(AdurasAnimHash.DodgeRHash, dodgdeDir.x);
            _SMch.Animator.SetFloat(AdurasAnimHash.DodgeFHash, dodgdeDir.y);
            if (dodgdeDir == Vector2.zero)
            {
                _SMch.Animator.SetFloat(AdurasAnimHash.DodgeFHash, 1);
                dodgdeDir = Vector2.up;
            }
            _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.DodgeBlendTreeHash, 0.1f);

        }
        else
        {
            _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.DodgeHash, 0.1f);
        }
    }

    public override void _OnExit()
    {
    }

    public override void _OnUpdate(float tick)
    {
        Vector3 dodgeMovement = new Vector3();
        if (_SMch.EnterCombatMode)
        {
            dodgeMovement = _SMch.transform.forward * dodgdeDir.y + _SMch.transform.right * dodgdeDir.x;
            dodgeMovement.y = 0;
        }
        else
        {
            dodgeMovement = _SMch.CharacterController.transform.forward;
            dodgeMovement.y = 0;
        }
        // dodgeMovement += _SMch.transform.right * dodgdeDir.x * tick;
        AdurasMove(dodgeMovement * dodgeSpeed, tick);

        dogdeDuration -= tick;
        if (dogdeDuration <= 0f)
        {
            if (_SMch.CharacterController.isGrounded)
            {
                _SMch._SwitchState(new PlayerFreeLookState(_SMch));
                return;
            }
            else
            {
                _SMch._SwitchState(new PlayerFallingState(_SMch));
                return;
            }
        }

    }
}
