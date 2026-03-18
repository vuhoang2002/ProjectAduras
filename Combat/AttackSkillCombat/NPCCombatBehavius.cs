using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static cbValue;

public class NPCCombatBehavius : MonoBehaviour
{
    public AICombatBehaviour AICombatBehaviour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [field: SerializeField] public List<SkillData> closeSkill = new List<SkillData>();
    [field: SerializeField] public List<SkillData> rangeSkill = new List<SkillData>();
    [field: SerializeField] public List<SkillData> defendSkill = new List<SkillData>();
    CombatController cc;
    public int closeScore;
    public int rangeScore;
    private int defendScore;
    private Vector2 randomScore = new Vector2(0, 5);
    private int distanceScore = 5;

    void Start()
    {
        cc = GetComponent<CombatController>();
        ClassifySkills(this.cc.combatHandData);

    }
    void Update()
    {
    }
    void ClassifySkills(CombatHandData data)
    {
        foreach (SkillData skill in GetAllSkills(data))
        {
            if (skill == null) continue;

            switch (skill.attackProperty)
            {
                case AttackProperty.Melee:
                    closeSkill.Add(skill); break;
                case AttackProperty.Shooter:
                    rangeSkill.Add(skill);
                    break;
                case AttackProperty.MeleeRanger:
                    rangeSkill.Add(skill);
                    closeSkill.Add(skill); break;
                case AttackProperty.Shield: defendSkill.Add(skill); break;
            }
        }
    }
    IEnumerable<SkillData> GetAllSkills(CombatHandData data)
    {
        yield return data.baseSkill_PHand;
        yield return data.Q_Skill_PHand;
        yield return data.E_Skill_PHand;

        yield return data.baseSkill_SHand;
        yield return data.Q_Skill_SHand;
        yield return data.E_Skill_SHand;
    }
    public SkillData CaculateScore(float distance)
    {
        if (distance <= this.AICombatBehaviour.closeDistance) closeScore += distanceScore;
        else rangeScore += distanceScore;
        closeScore = AddRandomScore(closeScore) + AICombatBehaviour.closeScore;
        rangeScore = AddRandomScore(rangeScore) + AICombatBehaviour.rangerScore;
        int max = Mathf.Max(closeScore, rangeScore, defendScore);
        if (max == closeScore) return UseClose();
        else if (max == rangeScore) return UseRange();
        return UseClose();
    }
    private int AddRandomScore(int score)
    {
        return score += Mathf.RoundToInt(Random.Range(randomScore.x, randomScore.y));
    }
    private SkillData UseClose()
    {
        int randomCloseSkill = Random.Range(0, closeSkill.Count);
        Debug.Log("Skill chọn là close: " + closeSkill[randomCloseSkill]);
        return closeSkill[randomCloseSkill];
    }
    private SkillData UseRange()
    {
        int randomRangeSkill = Random.Range(0, rangeSkill.Count);
        Debug.Log("Skill chọn là range: " + rangeSkill[randomRangeSkill]);
        return rangeSkill[randomRangeSkill];
    }
    public void ResetScore()
    {
        closeScore = 0;
        rangeScore = 0;
        defendScore = 0;
    }
}
