using UnityEngine;
using System.Collections.Generic;

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance;

    private Dictionary<GameObject, Queue<GameObject>> pool = new();

    void Awake()
    {
        Instance = this;
    }

    public GameObject Spawn(GameObject prefab, Transform parent)
    {
        if (!pool.ContainsKey(prefab))
            pool[prefab] = new Queue<GameObject>();

        if (pool[prefab].Count > 0)
        {
            GameObject obj = pool[prefab].Dequeue();
            obj.SetActive(true);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            return obj;
        }

        return Instantiate(prefab, parent);
    }

    public void Despawn(GameObject obj, GameObject prefab)
    {
        obj.SetActive(false);
        pool[prefab].Enqueue(obj);
    }
}