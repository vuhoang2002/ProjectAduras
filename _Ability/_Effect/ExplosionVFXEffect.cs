using UnityEngine;
[CreateAssetMenu(
    fileName = "ExplosionVFXEffect",
    menuName = "ImpactEffect/Explosion"
)]

public class ExplosionVFXEffect : EffectDefinition
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject vfxEffect;
    public float timeToDie;

    public override void Destroy()
    {

    }

    public override void Excute(SkillContext ctx)
    {

        var vfx = Instantiate(vfxEffect, ctx.impactPoint, Quaternion.identity);
        Destroy(vfx, timeToDie);
    }
}
