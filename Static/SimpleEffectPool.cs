using System.Collections.Generic;
using UnityEngine;

public class SimpleEffectPool : MonoBehaviour
{
    private static Dictionary<GameObject, Queue<GameObject>> pool = new();

    public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if (!pool.ContainsKey(prefab))
            pool[prefab] = new Queue<GameObject>();

        GameObject obj;

        if (pool[prefab].Count > 0)
        {
            obj = pool[prefab].Dequeue();
            obj.transform.SetPositionAndRotation(pos, rot);
            obj.transform.SetParent(parent);
            obj.SetActive(true);
        }
        else
        {
            obj = GameObject.Instantiate(prefab, pos, rot, parent);
            obj.AddComponent<PooledEffect>().prefab = prefab;
        }
        if (obj.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
        {
            ps.Play();
        }
        return obj;
    }

    public static void Despawn(GameObject obj, GameObject prefab)
    {
        obj.SetActive(false);
        pool[prefab].Enqueue(obj);
    }
}