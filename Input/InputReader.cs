using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static cbValue;

public class InputReader : AdurasMonobehavius, Controls.IPlayerInputActions
{
    private Controls _controls;
    public Vector2 VectorMovement { get; private set; } = Vector2.zero;
    public event Action PressPrimaryHand;
    public event Action ReleasePrimaryHand;
    public event Action PressSecondaryHand;
    public event Action ReleaseSecondaryHand;
    public event Action JumpEvent;
    public bool isSprinting { get; set; } = false;
    public event Action DodgeEvent;
    public bool IsPActive { get; private set; } = false;//primary hand active 
    public bool IsSActive { get; private set; } = false;//secondary hand active
    public bool IsQSkill { get; private set; } = false;//Q Skill active
    public bool IsESkill { get; private set; } = false;//E Skill active
    private CbSkill cbSkill = CbSkill.None;
    [field: SerializeField] public float pHoldTime { get; private set; } = 0f;
    public float sHoldTime { get; private set; } = 0f;
    public CbSkill CbSkill
    {
        get { return cbSkill; }
        set
        {
            setCbSkill();
        }
    }
    /// <summary>
    /// call to PlayerInput of  Input Action Map "Controls" that i created in Input System
    /// </summary>
    void Start()
    {
        _controls = new Controls();
        _controls.PlayerInput.SetCallbacks(this);
        _controls.PlayerInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPActive)
        {
            pHoldTime += Time.deltaTime;
        }
        else
        {
            pHoldTime = 0f;
        }
        if (IsSActive)
        {
            sHoldTime += Time.deltaTime;
        }
        else
        {
            sHoldTime = 0f;
        }
    }
    public void OnLeftM(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsPActive = true;
            PressPrimaryHand?.Invoke();
        }
        else if (context.canceled)
        {
            IsPActive = false;
            ReleasePrimaryHand?.Invoke();
        }
    }
    public void OnRightM(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsSActive = true;
            PressSecondaryHand?.Invoke();
        }
        else if (context.canceled)
        {
            IsSActive = false;
            ReleaseSecondaryHand?.Invoke();
        }
    }


    public void OnLook(InputAction.CallbackContext context)
    {

    }

    public void OnMove(InputAction.CallbackContext context)
    {

        VectorMovement = context.ReadValue<Vector2>();
        if (context.canceled)// đi lùi ko tính là đi nhanh || VectorMovement.y < 0&& enteringcombatmode
        {
            isSprinting = false;
        }
    }

    public void OnSkillQ(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsQSkill = true;
            setCbSkill();
        }
        else if (context.canceled)
        {
            IsQSkill = false;
            setCbSkill();
        }
    }

    public void OnSkillE(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsESkill = true;
            setCbSkill();
        }
        else if (context.canceled)
        {
            IsESkill = false;
            setCbSkill();
        }
    }

    void setCbSkill()
    {
        if (IsQSkill && IsESkill)
        {
            cbSkill = CbSkill.Q_Eskill;
        }
        else if (IsQSkill)
        {
            cbSkill = CbSkill.Qskill;

        }
        else if (IsESkill)
        {
            cbSkill = CbSkill.Eskill;
        }
        else if (!IsQSkill && !IsESkill)
        {
            cbSkill = CbSkill.None;
        }

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            JumpEvent?.Invoke();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (context.interaction is HoldInteraction)
            {
                isSprinting = true;
            }
            else if (context.interaction is TapInteraction)
            {
                DodgeEvent?.Invoke();
            }
        }
    }
    public void resetPress()
    {
        pHoldTime = 0f;
        sHoldTime = 0f;
    }
}
