using UnityEngine;

[CreateAssetMenu(
    fileName = "SpawnEffect",
    menuName = "SpawnEffect/SpawnEffect"
)]
public class SpawnEffect : EffectDefinition
{
    public GameObject prefabs;
    public float lifeTime = 1f;
    public bool isChild = false;
    public bool autoDestroy = false;

    private GameObject effect;

    public override void Excute(SkillContext ctx)
    {
        if (isChild)
        {
            effect = SimpleEffectPool.Spawn(
                prefabs,
                ctx.castPos.position,
                Quaternion.identity,
                ctx.castPos
            );
        }
        else
        {
            Vector3 dir = ctx.castPos.forward;
            dir.y = 0f;
            dir.Normalize();

            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

            effect = SimpleEffectPool.Spawn(
                prefabs,
                ctx.castPos.position,
                rot
            );
        }

        ctx.spawnedEffects.Add(effect);

        if (autoDestroy && lifeTime > 0f)
        {
            PooledEffect p = effect.GetComponent<PooledEffect>();
            if (p != null)
                p.Despawn(lifeTime);
        }
    }

    public void DestroyEffect()
    {
        if (effect != null)
        {
            PooledEffect p = effect.GetComponent<PooledEffect>();
            if (p != null)
                p.Despawn(lifeTime);
        }
    }

    public override void Destroy()
    {
        if (effect != null)
        {
            PooledEffect p = effect.GetComponent<PooledEffect>();
            if (p != null)
                p.Despawn();
        }
    }
}