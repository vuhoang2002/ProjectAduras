using Unity.VisualScripting;
using UnityEngine;
using static cbValue;
public class WeaponHit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject hitEffectPrefab;
    public GameObject hitGround;
    public WeaponData weaponData;
    public BoxCollider weaponCollider;
    public int weaponDamage;
    public int finalDamage;
    public Character attacker;
    private DamageInfo info;
    void Start()
    {
        weaponDamage = weaponData.damageBasic;
        if (weaponCollider == null)
        {
            weaponCollider = GetComponent<BoxCollider>();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Va chamj vowsi" + other.name);
        if (other.tag == "Enemy" || other.tag == "Player")
        {

            // Debug.Log("Hit an enemy: " + other.name);
            CreateHitEffect(other.ClosestPoint(transform.position), Vector3.up);
            info._impactPos = other.ClosestPoint(transform.position);
            //nếu đang guard

            // Xử lý sát thương ở đây nếu cần
            Character target = other.GetComponent<Character>();

            finalDamage = CombatProcessor.ProcessHit(
                attacker,
                target,
                info
            );
            if (finalDamage != 0)
                other.GetComponent<Health>()?.TakeDamage(finalDamage, info);
        }
        // if (other.tag == "Enviroment")
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstract"))
        {
            CreateHitEffectObstract(other.ClosestPoint(transform.position), Vector3.up);
        }
    }



    void CreateHitEffect(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
            // float lifetime = effect.GetComponent<ParticleSystem>().main.duration;
            Destroy(effect, 0.3f); // tự hủy sau thời gian tồn tại của hiệu ứng
        }
    }
    void CreateHitEffectObstract(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitGround, hitPoint, Quaternion.LookRotation(hitNormal));
            // float lifetime = effect.GetComponent<ParticleSystem>().main.duration;
            Destroy(effect, 0.3f); // tự hủy sau thời gian tồn tại của hiệu ứng
        }
    }
    public void EnableCollider(bool enable, DamageInfo info)
    {
        // Collider weaponCollider = GetComponent<Collider>();
        if (enable)
        {
            info._rawDamage = Mathf.RoundToInt(weaponDamage * info._damagePercent);

            this.info = info;
            //int damageCause = 12;
        }
        if (weaponCollider != null)
        {
            weaponCollider.enabled = enable;
        }
    }
    public void EnableCollider(bool enable)
    {

        if (weaponCollider != null)
        {
            weaponCollider.enabled = enable;
        }
    }
    public void setAttacker(Character character)
    {
        this.attacker = character;
    }

}
