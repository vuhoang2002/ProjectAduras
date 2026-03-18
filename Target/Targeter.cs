using System.Collections.Generic;
using UnityEngine;

public class Targeter : AdurasMonobehavius
{
    public static Targeter Instance;
    public List<Target> targets = new List<Target>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) { return; }
        targets.Add(target);
    }
    void OnTriggerExit(Collider other)
    {
        Target target = other.GetComponent<Target>();
        RemoveEnemyInRange(target);

    }
    public void RemoveEnemyInRange(Target enemy)
    {
        if (enemy == null)
        {
            return;
        }
        if (targets.Contains(enemy))
        {
            targets.Remove(enemy);
        }
    }
}
