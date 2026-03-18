using System.Collections.Generic;
using UnityEngine;
using static cbValue;
//skill context là nơi lưu trữ tất cả thông tin liên quan đến việc sử dụng chiêu, bao gồm vị trí xuất chiêu, mục tiêu, điểm va chạm, vận tốc ban đầu, cấp độ chiêu, hệ số sát thương và định nghĩa chiêu. Khi một chiêu được sử dụng, một đối tượng SkillContext sẽ được tạo ra và truyền vào các hiệu ứng để chúng có thể truy cập thông tin cần thiết để thực hiện hành động của mình.
public class SkillContext
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform caster;// nguoi sử dụng chiêu
    public Transform castPos;// nơi xuất chiêu
    public GameObject target;//nơi chiêu bay đến
    public Vector3 impactPoint;//nơi chiêu va chạm với vật thể
    public Vector3 initialVelocity;//vận tốc ban đầu của chiêu, dùng cho projectile
    public float damageMultiplier;
    public SkillDefinition SkillDefinition;//hiệu ứng chiêu vaf thoonsg  soos
    public AttackData AttackData;//dữ liệu về đòn đánh, bao gồm phần trăm sát thương, hệ nguyên tố, loại sát thương, hiệu ứng trạng thái và dữ liệu lực đẩy
    public List<GameObject> spawnedEffects = new List<GameObject>();
}
