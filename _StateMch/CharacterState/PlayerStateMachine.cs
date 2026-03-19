using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static cbValue;
public class PlayerStateMachine : StateMachine
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InputReader InputReader { get; private set; }
    public Targeter Targeter;



    [field: SerializeField] public float rotationSmoothValue { get; private set; } = 10f;


    [field: SerializeField] public float JumpForce { get; private set; } = 7f;
    public DoubleJump doubleJump = DoubleJump.None;
    private float currentWeight = 0;//to change weight layer animator for cool

    public Camera mainCamera;

    public string curentState;
    RuntimeAnimatorController baseAnimator;
    [Header("Target Assist")]
    private float targetAssistRadius = 8f;
    private float targetAssistAngle = 70f;
    public LayerMask enemyLayer;




    void Awake()
    {

    }
    protected override void Start()
    {
        base.Start();
        baseAnimator = Animator.runtimeAnimatorController;
        InputReader = GetComponent<InputReader>();

        mainCamera = Camera.main;

        _SwitchState(new PlayerFreeLookState(this));
    }
    public void RevertToBaseAnimatorController()
    {
        Animator.runtimeAnimatorController = baseAnimator;
    }




    IEnumerator ChangeWeightMaskLayer(int layer, float targetWeight, float speed)
    {

        //currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 2);
        // Animator.SetLayerWeight(1, currentWeight);
        float currentWeight = Animator.GetLayerWeight(layer);

        while (!Mathf.Approximately(currentWeight, targetWeight))
        {
            currentWeight = Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime * speed);
            Animator.SetLayerWeight(layer, currentWeight);
            yield return null; // chờ frame tiếp theo
        }

    }
    public void ChangeWeight(int layer, float a, float speed)
    {
        StartCoroutine(ChangeWeightMaskLayer(layer, a, speed));
    }
    private Vector3 lastPosition;
    void Update()
    {
        base.Update();

    }
    void AnimBySpeed()
    {
        var deltaPos = transform.position - lastPosition;
        var velocity = deltaPos / Time.deltaTime;

        float forwardSpeed = Vector3.Dot(transform.forward, velocity);
        Animator.SetFloat(AdurasAnimHash.MoveAmountHash, (forwardSpeed / runSpeed), 0.2f, Time.deltaTime);

        //  float angle = Vector3.SignedAngle(velocity, transform.forward, Vector3.up);
        //float sideSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
        float sideSpeed = Vector3.Dot(transform.right, velocity);
        Animator.SetFloat(AdurasAnimHash.HorizontalHash, (sideSpeed / runSpeed), 0.2f, Time.deltaTime);

        lastPosition = transform.position;
    }
    public void RotateTowardCameraForward(float deltaTime)
    {
        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0f; // bỏ thành phần Y để không bị ngửa/nghiêng
        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * deltaTime);
    }
    public float getMoveSpeed()
    {
        if (InputReader.isSprinting)
        {
            return runSpeed;
        }
        else
        {
            return walkSpeed;
        }
    }
    public float getAnimBySpeed()
    {
        if (InputReader.isSprinting)
        {
            if (Character.isSpeedBuff) return 1.5f;
            return 1f;
        }
        else
            return 0.5f;
    }
    public Vector2 getValueAnimSpeed(Vector2 valueMovement)
    {
        float valueX = valueMovement.x;
        float valueY = valueMovement.y;
        float outPx = 0;
        float outPy = 0;
        if (valueX > 0)
            if (InputReader.isSprinting)
            {
                outPx = 1;
            }
            else
            {
                outPx = 0.5f;
            }
        if (valueX < 0)
            if (InputReader.isSprinting)
            {
                outPx = -1;
            }
            else
            {
                outPx = -0.5f;
            }
        if (valueY > 0)
            if (InputReader.isSprinting)
            {
                outPy = 1;
            }
            else
            {
                outPy = 0.5f;
            }
        if (valueY < 0)
            if (InputReader.isSprinting)
            {
                outPy = -1;
            }
            else
            {
                outPy = -0.5f;
            }
        return new Vector2(outPx, outPy);
    }
    public Transform FindBestTarget()
    {
        // Collider[] hits = Physics.OverlapSphere(transform.position, targetAssistRadius, enemyLayer);
        if (Targeter.targets.Count == 0)
            return null;
        List<Target> hits = Targeter.targets;
        Transform bestTarget = null;
        float bestScore = -999f;

        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0;

        foreach (var hit in hits)
        {
            Vector3 dir = hit.transform.position - transform.position;
            float distance = dir.magnitude;

            dir.Normalize();

            float dot = Vector3.Dot(camForward, dir);

            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (angle > targetAssistAngle) continue;

            float score = dot * 2f - distance * 0.1f;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = hit.transform;
            }
        }

        //  currentTarget = bestTarget;
        SetTarget(bestTarget);
        return bestTarget;
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
    public Vector3 CalculateMovementByLook()
    {
        Vector2 inputVector = InputReader.VectorMovement;
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        //hướng của camera nhân với 1 hoặc -1 tùy theo input
        Vector3 movement = forward * inputVector.y + right * inputVector.x;

        return movement.normalized;
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
        // _SwitchState(new EnemyIdleState(this));
    }
}
