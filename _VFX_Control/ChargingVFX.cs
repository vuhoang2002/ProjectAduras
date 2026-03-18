using UnityEngine;

public class ChargingVFX : MonoBehaviour
{
    GameObject prefab;

    public void Init(GameObject p)
    {
        prefab = p;
    }

    public void Stop()
    {
        EffectPool.Instance.Despawn(gameObject, prefab);
    }
}