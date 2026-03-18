using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static cbValue;
public class Health : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int MAX_HEALTH = 100;
    private StateMachine stateMachine;
    public Image _HealthBarSprite;
    public Image _StaminaBarSprite;
    public Image _MagikaBarSprite;
    private Camera mainCamera;
    private float reduceSpeed = 2f;
    private float targetSlideValue = 1;
    private Character character;
    public StatResource HP;
    public StatResource MP;
    public StatResource SP;
    private float recoverInterval = 0.3f; // Thời gian giữa các lần hồi phục
    private float timer = 0f; // Biến đếm thời gian
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        character = GetComponent<Character>();
        mainCamera = Camera.main;
        HP = new StatResource(character.Stats.MaxHP);
        SP = new StatResource(character.Stats.MaxStamina);
        MP = new StatResource(character.Stats.MaxMana);
        MAX_HEALTH = HP.max;


    }
    void Update()
    {
        targetSlideValue = slideBar(MAX_HEALTH, HP.current);
        _HealthBarSprite.fillAmount = Mathf.MoveTowards(_HealthBarSprite.fillAmount, targetSlideValue, reduceSpeed * Time.deltaTime);
        if (_StaminaBarSprite != null)
        {
            _StaminaBarSprite.fillAmount = Mathf.MoveTowards(_StaminaBarSprite.fillAmount, slideBar(SP.max, SP.current), reduceSpeed * Time.deltaTime);
        }
        if (_MagikaBarSprite != null)
        {
            _MagikaBarSprite.fillAmount = Mathf.MoveTowards(_MagikaBarSprite.fillAmount, slideBar(MP.max, MP.current), reduceSpeed * Time.deltaTime);
        }

    }
    void FixedUpdate()
    {
        // Cộng dồn thời gian đã trôi qua
        timer += Time.fixedDeltaTime;

        // Kiểm tra điều kiện để hồi phục
        if (timer >= recoverInterval && (!stateMachine.isAttacking && !stateMachine.CombatController.isGuard))
        {
            SP.Recover(5);
            timer = 0f; // Reset timer
        }
    }

    public void TakeDamage(int finalDamage, DamageInfo info)
    {
        HP.current -= finalDamage;
        if (info._knockOnEffect != KnockOnEffect.None)
        {
            if (CompareTag("Enemy"))
                stateMachine._SwitchState(new EnemyHitState(stateMachine as EnemyStateMachine, info._knockOnEffect, info._force, info._impactPos));
            else if (CompareTag("Player"))
                stateMachine._SwitchState(new PlayerHitState(stateMachine as PlayerStateMachine, info._knockOnEffect, info._force, info._impactPos));
        }
        DamageEventSystem.Raise(finalDamage, info);
        if (HP.current <= 0)
        {
            HP.current = 0;
            Die();
        }
    }
    public void Die()
    {
        Debug.Log("Character has died.");
        if (transform.CompareTag(AdurasLayer.Enemy))
        {
            EnemyManager.Instance.RemoveEnemyInRange(transform.GetComponent<EnemyStateMachine>());
            Targeter.Instance.RemoveEnemyInRange(transform.GetComponentInChildren<Target>());
            EnemyStateMachine enemyStateMachine = GetComponent<EnemyStateMachine>();
            enemyStateMachine.HealthBar?.SetActive(false);
            stateMachine._SwitchState(new EnemyDeathState(enemyStateMachine));
        }
        else if (transform.CompareTag(AdurasLayer.Player))
        {
            GetComponent<Ragdoll>().ToggleRagdoll(true);
            RemoveTargetInRange(stateMachine.CombatController);
            stateMachine._SwitchState(new PlayerDeathState(stateMachine as PlayerStateMachine));
        }
        this.gameObject.tag = AdurasLayer.Ragdoll;

        //Destroy(gameObject);
    }
    float slideBar(float maxHealth, float currentHealth)
    {
        targetSlideValue = currentHealth / maxHealth;
        return targetSlideValue;
    }
    private void RemoveTargetInRange(CombatController playerCbCtrler)
    {
        foreach (var enemy in EnemyManager.Instance.enemyInRange)
        {
            if (enemy.Target == playerCbCtrler)
            {
                enemy.TargetInRange.Remove(playerCbCtrler);

            }
        }
    }


}
public class StatResource
{
    public int max;
    public int current;
    public StatResource(int max)
    {
        this.max = max;
        this.current = max;
    }
    public bool Consume(int amount)
    {
        if (current < amount) return false;

        current -= amount;
        return true;
    }
    public void Recover(float amount)
    {
        int amount2 = Mathf.FloorToInt(amount);
        current = Mathf.Min(max, current + amount2);
    }

}