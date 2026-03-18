using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
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

    public List<EnemyStateMachine> enemyInRange = new List<EnemyStateMachine>();
    float notAttackTimer = 0f;
    public Vector2 timeAttack = new Vector2(2f, 5f);
    public int maxAttacker = 2;
    void Start()
    {
        notAttackTimer = Random.Range(timeAttack.x, timeAttack.y);

    }

    // void Update()
    // {
    //     if (enemyInRange.Count <= 0)
    //         return;
    //     if (!enemyInRange.Any(e => e.IsInState(eCombatState.Attack) || e.IsInState(eCombatState.Hit) || e.IsInState(eCombatState.Recoil)))
    //     {
    //         if (notAttackTimer > 0f)
    //             notAttackTimer -= Time.deltaTime;
    //         if (notAttackTimer <= 0)
    //         {
    //             //attack the player :3
    //             var enemyWhoAttack = SelectEnemyToAttack();
    //             if (enemyWhoAttack != null)
    //             {
    //                 enemyWhoAttack._SwitchState(new EnemyAttackState(enemyWhoAttack));
    //                 notAttackTimer = Random.Range(timeAttack.x, timeAttack.y); //cooldown time between calling attacks
    //             }

    //         }
    //     }
    // }
    public void AddEnemyInRange(EnemyStateMachine enemy)
    {
        if (!enemyInRange.Contains(enemy))
        {
            enemyInRange.Add(enemy);
        }
    }
    public void RemoveEnemyInRange(EnemyStateMachine enemy)
    {
        if (enemyInRange.Contains(enemy))
        {
            enemyInRange.Remove(enemy);
        }
    }
    // EnemyStateMachine SelectEnemyToAttack()
    // {
    //     // Chỉ chọn enemy đã có target
    //     return enemyInRange
    //         .Where(e => e.Target != null)
    //         .OrderByDescending(e => e.CombatMovementTimer)
    //         .FirstOrDefault();
    // }
    void Update()
    {
        if (enemyInRange.Count <= 0)
            return;

        int currentAttacker = enemyInRange.Count(e =>
            e.IsInState(eCombatState.Attack));

        // Nếu chưa đủ số lượng attacker → cho thêm vào
        if (currentAttacker < maxAttacker)
        {
            if (notAttackTimer > 0f)
            {
                notAttackTimer -= Time.deltaTime;
            }
            else
            {
                var enemyWhoAttack = SelectEnemyToAttack();

                if (enemyWhoAttack != null)
                {
                    enemyWhoAttack._SwitchState(new EnemyAttackState(enemyWhoAttack));

                    // reset timer để tạo nhịp
                    notAttackTimer = Random.Range(timeAttack.x, timeAttack.y);
                }
            }
        }
    }
    EnemyStateMachine SelectEnemyToAttack()
    {
        var candidates = enemyInRange
            .Where(e => e.Target != null &&
            !e.IsInState(eCombatState.Attack) &&
            !e.IsInState(eCombatState.Death) &&
            !e.IsInState(eCombatState.Hit))
            .ToList();

        if (candidates.Count == 0) return null;

        // random nhẹ để không bị predictable
        return candidates
            .OrderByDescending(e => e.CombatMovementTimer + Random.Range(0f, 2f))
            .FirstOrDefault();
    }
}
