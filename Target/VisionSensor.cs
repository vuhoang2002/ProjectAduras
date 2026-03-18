using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    public EnemyStateMachine enemyStateMachine;
    void OnTriggerEnter(Collider other)
    {
        var combat = other.GetComponent<CombatController>();
        if (combat != null)
        {
            enemyStateMachine.TargetInRange.Add(combat);
            EnemyManager.Instance.AddEnemyInRange(enemyStateMachine);
        }
    }
    void OnTriggerExit(Collider other)
    {
        var combat = other.GetComponent<CombatController>();
        if (combat != null)
        {
            enemyStateMachine.TargetInRange.Remove(combat);
            EnemyManager.Instance.RemoveEnemyInRange(enemyStateMachine);
        }
    }
    public void RemoveTarget(CombatController combatController)
    {
        enemyStateMachine.TargetInRange.Remove(combatController);
    }
}
