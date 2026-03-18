using UnityEngine;

public class PooledEffect : MonoBehaviour
{
    public GameObject prefab;

    public void Despawn(float lifeTime)
    {
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    public void Despawn()
    {
        ReturnToPool();
    }

    void ReturnToPool()
    {
        SimpleEffectPool.Despawn(gameObject, prefab);
    }
}