using UnityEngine;

public static class SkillImpactSystem
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static void ApplyImpact(SkillContext ctx)
    {
        //  ctx.impactPoint = ctx.target.transform.position;
        foreach (var effect in ctx.SkillDefinition.effects)
        {
            if (effect is ImpactEffect)
            {
                effect.Excute(ctx);
            }
        }
    }
}
