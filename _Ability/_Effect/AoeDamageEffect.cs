using UnityEngine;
using static cbValue;
[CreateAssetMenu(
    fileName = "AoeEffect",
    menuName = "ImpactEffect/AoeEffect"
)]

public class AoeDamageImpactEffect : EffectDefinition
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float radius;
    public int damage;

    public override void Destroy()
    {
    }

    public override void Excute(SkillContext ctx)
    {
        Collider[] hits = Physics.OverlapSphere(ctx.impactPoint, radius);

        foreach (var hit in hits)
        {
            hit.TryGetComponent(out Character target);
            Character attacker = ctx.caster.GetComponent<Character>();
            Debug.Log($"DamageEffectAOE hit: {hit?.name}+ attacker: {attacker?.name}");
            if (target == null || attacker == null)
                continue;
            DamageInfo damageInfo = new DamageInfo
            {
                _rawDamage = ctx.SkillDefinition.otherDamage,
                _damagePercent = ctx.AttackData.DamagePercent[0],//tạm thời lấy phần tử đầu tiên, sau này sẽ tùy theo impact count
                _Type = ctx.AttackData.AttackDamageType,
                _schoolType = ctx.AttackData.AttackElemental,
                _effectStatus = ctx.AttackData.EffectStatus,
                _statScale = ctx.AttackData.statPri,
                _impactPos = ctx.impactPoint

            };
            if (ctx.AttackData.forceData.Count != 0)
            {
                damageInfo._force = ctx.AttackData.forceData[0].forceDirection;
                damageInfo._knockOnEffect = ctx.AttackData.forceData[0].knockOnEffect;
            }
            int finalDamage = CombatProcessor.ProcessHit(
                    attacker,
                     target,
                      damageInfo,
                      false
        );
            if (finalDamage > 0)
                target.GetComponent<Health>()?.TakeDamage(finalDamage, damageInfo);
            else
            {
                ctx.caster.GetComponent<CombatController>().DestroyEffect(ctx);
            }
        }
    }
}
