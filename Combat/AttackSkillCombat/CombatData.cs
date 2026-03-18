using UnityEngine;
[CreateAssetMenu(fileName = "NewCombatHandData", menuName = "_ScriptableObjects/CombatHandData")]
public class CombatHandData : ScriptableObject
{
    #region PrimaryHand
    [Header("PrimarySkil")]
    public SkillData baseSkill_PHand;
    public SkillData Q_Skill_PHand;
    public SkillData E_Skill_PHand;
    #endregion
    #region SecondaryHand
    [Header("SecondarySkill")]
    public SkillData baseSkill_SHand;
    public SkillData Q_Skill_SHand;
    public SkillData E_Skill_SHand;
    #endregion
    // #region EightSpecialAbility
    // [Header("EightSpecialSkill")]

    // #endregion

}
