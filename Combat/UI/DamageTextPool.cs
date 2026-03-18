using System.Collections.Generic;
using UnityEngine;

public class DamageTextPool : MonoBehaviour
{
    public static DamageTextPool Instance;

    [SerializeField] DamageTextUI prefab;

    Queue<DamageTextUI> pool = new Queue<DamageTextUI>();

    void Awake()
    {
        Instance = this;
    }

    public DamageTextUI Get()
    {
        DamageTextUI obj = pool.Count > 0
            ? pool.Dequeue()
            : Instantiate(prefab, transform);

        obj.gameObject.SetActive(true);
        obj.ResetState();

        return obj;
    }

    public void Return(DamageTextUI obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}