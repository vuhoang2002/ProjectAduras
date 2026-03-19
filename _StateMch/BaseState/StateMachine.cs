using UnityEngine;

public class StateMachine : AdurasMonobehavius
{
    public State _currentState;

    public Animator Animator;
    public bool isAttacking { get; set; } = false;
    protected bool rootMotion = false;
    public bool _RootMotion
    {
        get { return rootMotion; }
        set
        {
            rootMotion = value;
            Animator.applyRootMotion = value;
        }
    }
    [field: SerializeField] public CombatController CombatController { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public bool isGrounded { get; private set; } = true;
    private Vector3 groundCheckOffset = new Vector3(0, 0f, 0);
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.3f;
    public Transform currentTarget;
    public Health targetHealth;
    private bool enterCombat;
    public bool EnterCombatMode
    {
        get { return enterCombat; }
        set
        {
            enterCombat = value;
            EnterCombatModeFun(value);
        }
    }
    [field: SerializeField] public float runSpeed { get; set; } = 2f;
    [field: SerializeField] public float walkSpeed { get; set; } = 1f;
    public Health Health { get; private set; }
    public Character Character { get; private set; }
    public Ragdoll Ragdoll { get; private set; }
    [field: SerializeField] public CharacterController CharacterController { get; private set; }


    protected virtual void Start()
    {
        Animator = GetComponent<Animator>();
        CombatController = GetComponent<CombatController>();
        ForceReceiver = GetComponent<ForceReceiver>();
        Health = GetComponent<Health>();
        Character = GetComponent<Character>();
        groundLayer = LayerMask.GetMask("Obstract");
        Ragdoll = GetComponent<Ragdoll>();
        CharacterController = GetComponent<CharacterController>();

    }
    public void _SwitchState(State newState)
    {
        // Printf($"Switching to {newState.GetType().Name}");
        _currentState?._OnExit();

        _currentState = newState;

        _currentState?._OnEnter();
    }
    private float guardLayerWeight;
    private float priLayerWeight;
    protected void Update()
    {
        _currentState?._OnUpdate(Time.deltaTime);
        GroundCheck();
        guardLayerWeight = Mathf.Lerp(
               guardLayerWeight,
               CombatController.isGuard ? 1f : 0f,
                Time.deltaTime * 10f);
        Animator.SetLayerWeight(1, guardLayerWeight);
        priLayerWeight = Mathf.Lerp(
               priLayerWeight,
               CombatController.IsStanding ? 1f : 0f,
                Time.deltaTime * 10f);
        Animator.SetLayerWeight(2, priLayerWeight);
    }
    void GroundCheck()
    {
        Vector3 checkPosition = transform.position + groundCheckOffset;
        isGrounded = Physics.CheckSphere(checkPosition, groundCheckRadius, groundLayer);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }
    public void EnterCombatModeFun(bool enable)
    {
        if (enable)
        {
            Animator.CrossFade(AdurasAnimHash.CombatBlendTreeHash, 0.2f);
        }
        else
        {
            Animator.CrossFade(AdurasAnimHash.FreeLookBlendTreeHash, 0.2f);
        }
    }


}
