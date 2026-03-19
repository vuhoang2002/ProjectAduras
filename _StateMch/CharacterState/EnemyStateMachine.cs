using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[SerializeField]
public enum eCombatState
{
    Idle,
    Ready, // can attack or chase or cirling
    Chase,
    Circling,
    Attack,
    RetreatAfterAttack,
    Hit,
    Recoil,
    Guard,
    Death
}
public class EnemyStateMachine : StateMachine
{
    [field: SerializeField] public float Fov { get; private set; } = 180;
    public List<CombatController> TargetInRange = new List<CombatController>();

    public float MoveSpeed { get; private set; } = 2f;
    public float WalkSpeed { get; private set; } = 1.2f;
    public CombatController Target;
    public NavMeshAgent Agent { get; private set; }
    public Animator Animator { get; private set; }
    public string infomation;
    public eCombatState eCbBehavius;
    public float CombatMovementTimer { get; set; } = 0f;
    public float speedAnim = 0.7f;
    public LayerMask playerLayer;
    public GameObject HealthBar;


    void Start()
    {
        base.Start();
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = MoveSpeed;
        // Agent.updateRotation = true;
        // Agent.updatePosition = true;

        _SwitchState(new EnemyIdleState(this));

    }
    Vector3 lastPosition;
    void Update()
    {
        base.Update();
        AnimBySpeed();
    }
    void AnimBySpeed()
    {
        if (_RootMotion)
        {
            return;
        }
        var deltaPos = transform.position - lastPosition;
        var velocity = deltaPos / Time.deltaTime;

        float forwardSpeed = Vector3.Dot(transform.forward, velocity);
        Animator.SetFloat(AdurasAnimHash.MoveAmountHash, forwardSpeed / Agent.speed, 0.2f, Time.deltaTime);

        // float angle = Vector3.SignedAngle(velocity, transform.forward, Vector3.up);
        // float sideSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
        float sideSpeed = Vector3.Dot(transform.right, velocity);
        Animator.SetFloat(AdurasAnimHash.HorizontalHash, sideSpeed, 0.2f, Time.deltaTime);

        lastPosition = transform.position;
    }
    public bool IsInState(eCombatState state)
    {
        return eCbBehavius == state;
    }

    public void FindBestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 3f, playerLayer);

        //  List<Target> hits = Targeter.targets;
        Transform bestTarget = null;
        float bestScore = -999f;

        Vector3 camForward = transform.forward;
        camForward.y = 0;

        foreach (var hit in hits)
        {
            Vector3 dir = hit.transform.position - transform.position;
            float distance = dir.magnitude;

            dir.Normalize();

            float dot = Vector3.Dot(camForward, dir);

            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (angle > 80f) continue;

            float score = dot * 2f - distance * 0.1f;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = hit.transform;
            }
        }

        SetTarget(bestTarget);
    }
    public void AssistRotateToTarget(float tick)
    {
        if (currentTarget == null) return;

        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            tick * 12f
        );
    }
    public void SetTarget(Transform newTarget)
    {
        if (currentTarget != null)
        {
            currentTarget.TryGetComponent<Health>(out Health targetHealth);
            if (targetHealth != null)
            {
                targetHealth.OnDie -= OnTargetDie;
            }
        }
        currentTarget = newTarget;
        if (currentTarget != null)
        {
            targetHealth = currentTarget.GetComponent<Health>();

            if (targetHealth != null)
            {
                targetHealth.OnDie += OnTargetDie;
            }
        }
    }
    void OnTargetDie(Health deadTarget)
    {
        currentTarget = null;
        targetHealth = null;

        // chuyển state về idle / tìm target mới
        _SwitchState(new EnemyIdleState(this));
    }



}
