using System.Collections;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    public SkillDefinition skill;
    public GameObject kk;
    public void CastSkill(SkillDefinition skill, GameObject target)
    {
        SkillContext ctx = new SkillContext
        {
            castPos = gameObject.transform,
            target = target,
            impactPoint = target.transform.position,
            damageMultiplier = 1f,
            SkillDefinition = skill
        };
        foreach (var effect in skill.effects)
        {
            if (effect is ImpactEffect) continue;
            effect.Excute(ctx);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))

        {
            CastSkill(skill, kk);
        }

    }
}
