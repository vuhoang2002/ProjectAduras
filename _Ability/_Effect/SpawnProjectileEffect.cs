using System.Runtime.InteropServices;
using UnityEngine;
[CreateAssetMenu(
    fileName = "SpawnProjecttileEffect",
    menuName = "SpawnEffect/SpawnProjectileEffect"

)]
public class SpawnProjectileEffect : EffectDefinition
{
    public GameObject projectilePrefab;
    public float lifeTime = 5f;
    public float gravityWeight;
    public float speed = 5f;
    public bool destroyByImpact = true;

    public override void Destroy()
    {

    }

    public override void Excute(SkillContext ctx)
    {
        GameObject proj = Instantiate(projectilePrefab, ctx.castPos.transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        if (proj.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
        {
            ps.Play();
        }
        if (ctx.caster.CompareTag(AdurasLayer.Enemy))
        {
            proj.layer = LayerMask.NameToLayer(AdurasLayer.EnemyProjectile);
        }

        else if (ctx.caster.CompareTag(AdurasLayer.Player))
        {
            proj.layer = LayerMask.NameToLayer(AdurasLayer.PlayerProjectile);

        }
        projectile.SetInitialData(ctx, ctx.initialVelocity * speed, lifeTime, destroyByImpact);
        Destroy(proj, lifeTime);

    }

}
