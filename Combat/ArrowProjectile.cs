using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    private Vector3 currentVelocity;
    private Vector3 gravityVector;
    private bool isFlying = false;
    private float lifeTime;
    public LayerMask hitLayers; // gán trong Inspector, bỏ tick Player

    public void SetInitialData(Vector3 initialVelocity, Vector3 gravity, float firingTime)
    {
        currentVelocity = initialVelocity;
        gravityVector = gravity;
        lifeTime = firingTime;

        isFlying = true;

        // Hủy mũi tên sau firingTime nếu không trúng gì
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!isFlying) return;

        // Cập nhật vận tốc với trọng lực
        currentVelocity += gravityVector * Time.deltaTime;

        // Dịch chuyển
        Vector3 displacement = currentVelocity * Time.deltaTime;
        transform.position += displacement;

        // Xoay theo hướng bay
        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(currentVelocity);
        }

        // Kiểm tra va chạm
        CheckForHit(displacement.magnitude);
    }



    void CheckForHit(float moveDistance)
    {
        RaycastHit hit;
        Vector3 origin = transform.position - transform.forward * moveDistance;
        int hitLayers = ~LayerMask.GetMask("Player", "Ignore Raycast", "Default");
        // Chỉ raycast vào các layer được chọn trong hitLayers
        if (Physics.Raycast(origin, transform.forward, out hit, moveDistance, hitLayers))
        {
            isFlying = false;
            HandleHit(hit.collider, hit.point);
        }
    }

    void HandleHit(Collider collider, Vector3 hitPoint)
    {
        Debug.Log("Mũi tên trúng: " + collider.name + " tại " + hitPoint);

        transform.SetParent(collider.transform);
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 5f); // hủy sau 5 giây kể từ khi trúng
    }
}