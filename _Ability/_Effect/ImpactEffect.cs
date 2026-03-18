using UnityEngine;
[CreateAssetMenu]
public class ImpactEffect : EffectDefinition
{
    public EffectDefinition[] explosionEffect;

    public override void Destroy()
    {

    }

    public override void Excute(SkillContext ctx)
    {
        foreach (var effect in explosionEffect)
        {
            effect.Excute(ctx);
        }
    }
}
