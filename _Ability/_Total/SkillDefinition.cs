using UnityEngine;
using static cbValue;
[CreateAssetMenu(fileName = "SkillDefinition", menuName = "")]
public class SkillDefinition : ScriptableObject
{
    public string id;
    public string skillName;
    public int baseDamage;
    public int otherDamage;//cho vụ nổ vv...
    public float coolDown;
    public float castTime;
    public EffectDefinition[] effects;
    public Sprite icon;
    public string description;
    public SkillContext context;
    public SpawnEffectPos spawnEffectPos;
}
