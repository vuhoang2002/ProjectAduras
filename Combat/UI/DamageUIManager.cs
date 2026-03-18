using UnityEngine;
using static cbValue;

public class DamageUIManager : MonoBehaviour
{
    private void OnEnable()
    {
        DamageEventSystem.OnDamage += Spawn;
    }

    private void OnDisable()
    {
        DamageEventSystem.OnDamage -= Spawn;
    }
    public void Spawn(int finalDamage, DamageInfo info)
    {
        var text = DamageTextPool.Instance.Get();
        text.Setup(info._impactPos + Vector3.up * 0.2f, finalDamage, info.isCrit, info._Type);
    }
}