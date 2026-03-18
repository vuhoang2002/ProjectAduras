using UnityEngine;
using UnityEngine.AI;

public class ForceReceiver : MonoBehaviour
{
    //handle gravity
    [SerializeField] private CharacterController characterController;
    private float verticalVelocity;
    private Vector3 externalForce;
    public Vector3 movement => externalForce + Vector3.up * verticalVelocity;// use for gravity
    private Vector3 dampingVelocity;
    public Vector3 test;
    [SerializeField] private float drag = 0.1f;
    public NavMeshAgent agent;
    private bool isBeingForce;
    public float verticalVelocityTest = -0.5f;
    [field: SerializeField] public StateMachine StateMachine { get; private set; }
    void Start()
    {
        StateMachine = GetComponent<StateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        test = movement;
        if (verticalVelocity < 0f && StateMachine.isGrounded)
        {
            // verticalVelocity = Physics.gravity.y * Time.deltaTime;
            verticalVelocity = verticalVelocityTest;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
        // exponential decay mượt và tự nhiên hơn
        externalForce *= 1f - drag * Time.deltaTime;

        if (externalForce.magnitude < 0.05f)
        {
            externalForce = Vector3.zero;
        }
        // externalForce = Vector3.SmoothDamp(externalForce, Vector3.zero, ref dampingVelocity, drag * Time.deltaTime);

    }
    public Vector3 getMovement()
    {
        return movement;
    }
    public void Jump(float jumpForce)
    {
        verticalVelocity += jumpForce;
    }
    public void AddForce(Vector3 force)
    {
        externalForce += force;
        if (agent != null)
        {
            agent.enabled = false;
        }
    }
    public void ResetForces()
    {
        externalForce = Vector3.zero;
        if (agent != null)
        {
            agent.enabled = true;
        }
    }
    public bool IsBeingForced()
    {
        return externalForce.magnitude > 0.1f;
    }
}
