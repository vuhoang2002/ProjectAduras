using UnityEngine;

public class ThirdPersonShooter : MonoBehaviour
{
    [Header("Thiết lập Bắn")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float targetDistance = 5f;
    public float firingTime = 1.0f;
    public Animator characterAnimator; // Kéo component Animator vào đây trong Inspector
    public HumanBodyBones leftHandBone = HumanBodyBones.LeftHand;

    [Header("Tham chiếu")]
    public Camera mainCamera;

    private Vector3 gravityVector; // Sẽ lấy từ Physics.gravity

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        firePoint = characterAnimator.GetBoneTransform(leftHandBone);

        // **LẤY VECTOR TRỌNG LỰC TỪ CÀI ĐẶT CỦA UNITY**
        gravityVector = Physics.gravity * 0.5f;
    }

    void Update()
    {
        // ... (Giữ nguyên phần này) ...
        if (Input.GetButtonDown("Fire1"))
        {
            FireArrow();
        }
    }

    void FireArrow()
    {
        Vector3 endPoint = GetAimTargetPoint();
        Vector3 initialVelocity = CalculateInitialVelocity(endPoint);

        GameObject arrowObject = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        ArrowProjectile arrowScript = arrowObject.GetComponent<ArrowProjectile>();

        if (arrowScript != null)
        {
            // Truyền vector trọng lực đã lấy từ Physics.gravity
            // arrowScript.SetInitialData(initialVelocity, gravityVector);
        }
    }

    // ... (Giữ nguyên các phương thức GetAimTargetPoint và CalculateInitialVelocity) ...
    // Công thức tính V0: V0 = (P_end - P_start) / T - 0.5 * G * T
    Vector3 CalculateInitialVelocity(Vector3 endPoint)
    {
        Vector3 startPoint = firePoint.position;

        // P_end - P_start
        Vector3 displacement = endPoint - startPoint;

        // (P_end - P_start) / T
        Vector3 velocityNoGravity = displacement / firingTime;

        // 0.5 * G * T
        Vector3 gravityComponent = 0.5f * gravityVector * firingTime;

        // Trả về V0
        return velocityNoGravity - gravityComponent;
    }
    Vector3 GetAimTargetPoint()
    {
        Vector3 targetPoint;
        // Khởi tạo Raycast từ Camera, đi qua trung tâm màn hình (hồng tâm)
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // Định nghĩa một LayerMask để loại trừ (Ignore) các layer không cần thiết (Ví dụ: Player)
        // LayerMask.GetMask("Player") sẽ tạo ra một mask chỉ chứa layer "Player"
        // ~LayerMask.GetMask("Player") sẽ ĐẢO MASK, tức là CHỌN TẤT CẢ layer NGOẠI TRỪ "Player"
        int layerMask = ~LayerMask.GetMask("Player", "Ignore Raycast"); // Loại trừ Layer Player và Layer Ignore Raycast

        // Thực hiện Raycast. Sử dụng layerMask để loại bỏ va chạm với Player.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Trúng vật thể: Điểm kết thúc là điểm va chạm
            targetPoint = hit.point;
            Debug.Log("🎯 Trúng vật thể: " + hit.collider.name + " tại " + targetPoint);
        }
        else
        {
            // Không trúng vật thể (hoặc chỉ trúng vật thể đã bị loại trừ trong LayerMask)
            // Lấy điểm giả định cách xa camera một khoảng (targetDistance)
            targetPoint = ray.GetPoint(targetDistance);
        }
        return targetPoint;
    }
}