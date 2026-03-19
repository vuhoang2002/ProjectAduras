using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 target;
    public float gravityWeight;
    public Vector3 currentVelocity;
    private Vector3 currentGravity;
    private float lifeTime;
    SkillContext context;
    private bool destroyByImpact;
    void Start()
    {

    }

    // Update is called once per frame
    public void SetInitialData(SkillContext ctx, Vector3 initialVelocity, float lifeTime, bool destroyByImpact)
    {
        currentVelocity = initialVelocity;
        currentGravity = Physics.gravity * gravityWeight;
        this.lifeTime = lifeTime;
        target = ctx.impactPoint;
        this.context = ctx;
        this.destroyByImpact = destroyByImpact;

    }
    void Update()
    {
        Fly();
        //     transform.position = Vector3.MoveTowards(
        //         transform.position,   // vị trí hiện tại
        //         target,               // vị trí mục tiêu
        //         10f * Time.deltaTime   // tốc độ di chuyển
        //     );
    }
    void FLyToTarget()
    {
        transform.position = Vector3.MoveTowards(
                transform.position,   // vị trí hiện tại
                target,               // vị trí mục tiêu
                10f * Time.deltaTime   // tốc độ di chuyển
            );
    }

    void Fly()
    {
        // Cập nhật vận tốc với trọng lực
        currentVelocity += currentGravity * Time.deltaTime;

        // Dịch chuyển
        Vector3 displacement = currentVelocity * Time.deltaTime;
        transform.position += displacement;

        // Xoay theo hướng bay
        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(currentVelocity);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log($"name is: trigger" + other.gameObject.name);
        if (other.CompareTag(AdurasLayer.Enemy) || other.CompareTag(AdurasLayer.Player))
            Impact(other.gameObject);
    }

    void Impact(GameObject other)
    {
        context.impactPoint = transform.position + transform.forward * -0.1f;
        context.target = other;
        SkillImpactSystem.ApplyImpact(context);
        if (destroyByImpact)
        {
            //  Destroy(gameObject);
            GetComponent<PooledEffect>().Despawn();
        }
    }

}
