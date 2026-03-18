using System;
using UnityEngine;
using static cbValue;

public class CombatGetter
{
    // get  attackdata from  skilldata and combatdata scriptable object
    CombatController combatController;

    SkillData skillDataP;
    SkillData skillDataS;
    SkillData QSkillDataP;
    SkillData ESkillDataP;
    SkillData QSkillDataS;
    SkillData ESkillDataS;

    public CombatGetter(CombatController combatController)
    {
        this.combatController = combatController;

        LoadCombatData(combatController.combatHandData);

    }

    private void LoadCombatData(CombatHandData combatHandData)
    {
        this.skillDataP = combatHandData.baseSkill_PHand;
        this.skillDataS = combatHandData.baseSkill_SHand;

        this.QSkillDataP = combatHandData.Q_Skill_PHand;
        this.QSkillDataS = combatHandData.Q_Skill_SHand;

        this.ESkillDataP = combatHandData.E_Skill_PHand;
        this.ESkillDataS = combatHandData.E_Skill_SHand;
    }
    public SkillData _getCurrentSkill(CbArm cbArm, CbSkill cbSkill)
    {
        switch (cbArm)
        {
            case CbArm.Primary:
                switch (cbSkill)
                {
                    case CbSkill.None:
                        return skillDataP;
                    case CbSkill.Qskill:
                        return QSkillDataP;
                    case CbSkill.Eskill:
                        return ESkillDataP;
                }
                break;
            case CbArm.Secondary:
                switch (cbSkill)
                {
                    case CbSkill.None:
                        return skillDataS;
                    case CbSkill.Qskill:
                        return QSkillDataS;
                    case CbSkill.Eskill:
                        return ESkillDataS;
                }
                break;
        }
        return skillDataP;
    }
    public SkillData _getDefaultSkill(CbArm cbArm)
    {
        return _getCurrentSkill(cbArm, CbSkill.None);
    }

    public AttackType GetAttackType(SkillData skillData)
    {
        return skillData.attackType;
    }
    public SkillDefinition GetSkillDefinition_Attack(SkillData skillData, int cbCount, int impactCount, bool baseOrSpecial)
    {

        if (baseOrSpecial)
        {

            if (cbCount >= skillData.BaseAtkDt.Count)
            {
                cbCount = skillData.BaseAtkDt.Count - 1;
            }
            if (skillData.BaseAtkDt[cbCount].skillDefinition.Count == 0 || skillData.BaseAtkDt[cbCount].skillDefinition.Count < impactCount)
            {
                return null;
            }
            return skillData.BaseAtkDt[cbCount].skillDefinition[impactCount];
        }
        else
        {

            if (cbCount >= skillData.SpecialAtkDt.Count)
            {
                cbCount = skillData.SpecialAtkDt.Count - 1;
            }
            if (skillData.SpecialAtkDt[cbCount].skillDefinition.Count == 0)
            {
                return null;
            }
            return skillData.SpecialAtkDt[cbCount].skillDefinition[impactCount];
        }

    }

    public AttackProperty GetAttackPropertyFromSKill(SkillData skillData)
    {
        return skillData.attackProperty;
    }

    public string getAnim_Release(SkillData skillData, int indexCb, int indexAnim, bool baseOrSpecial)
    {
        if (baseOrSpecial)
            return skillData.BaseAtkDt[indexCb].AnimRelease[indexAnim];
        else
        {
            return skillData.SpecialAtkDt[indexCb].AnimRelease[indexAnim];
        }
    }
    public string getAnim_Charging(SkillData skillData, int index)
    {
        if (skillData.attackType == AttackType.Charging || skillData.attackType == AttackType.Charge_Normal)
            return skillData.AnimCharging[index];
        else
        {
            Debug.LogError("_Do not have anim because your attack type not charging");
            return null;
        }
    }
    public string getAnim_LoopCharge(SkillData skillData, int index)
    {
        if (skillData.attackType == AttackType.Charging || skillData.attackType == AttackType.Charge_Normal)
            return skillData.AnimLoopCharge[index];
        else
        {
            Debug.LogError("_Do not have anim because your attack type not charging");
            return null;
        }
    }
    public float getAnim_ImpactTime(SkillData skillData, int indexCb, int indexImpact, bool baseOrSpecial)
    {
        if (baseOrSpecial)
            return skillData.BaseAtkDt[indexCb].ImpactTime[indexImpact];
        else
            return skillData.SpecialAtkDt[indexCb].ImpactTime[indexImpact];
    }
    public float getAnim_CoolDownTime(SkillData skillData, int indexCb, int indexImpact, bool baseOrSpecial)
    {
        if (baseOrSpecial)
            return skillData.BaseAtkDt[indexCb].ImpactTime[indexImpact];
        else
            return skillData.SpecialAtkDt[indexCb].ImpactTime[indexImpact];
    }
    public float getAnim_OverTime(SkillData skillData, int indexCb, bool baseOrSpecial)
    {
        if (baseOrSpecial)
            return skillData.BaseAtkDt[indexCb].OverTime;
        else
            return skillData.SpecialAtkDt[indexCb].OverTime;
    }
    public bool isAddCombo(SkillData skillData, int indexCb, bool baseOrSpecial)
    {
        if (baseOrSpecial)
            return skillData.BaseAtkDt[indexCb].ComboStatus == ComboStatus.Add;
        else
            return skillData.SpecialAtkDt[indexCb].ComboStatus == ComboStatus.Add;
    }
    public bool isResetCombo(SkillData skillData, int indexCb, bool baseOrSpecial)
    {
        if (baseOrSpecial)
            return skillData.BaseAtkDt[indexCb].ComboStatus == ComboStatus.Reset;
        else
            return skillData.SpecialAtkDt[indexCb].ComboStatus == ComboStatus.Reset;
    }
    public int getImpactCount(SkillData skillData, int indexCb, bool baseOrSpecial)
    {
        if (baseOrSpecial)
            return skillData.BaseAtkDt[indexCb].ImpactTime.Count;
        else
            return skillData.SpecialAtkDt[indexCb].ImpactTime.Count;
    }
    public float getDamagePercent(SkillData skillData, int indexCb, int indexDamage, bool baseOrSpecial)
    {
        if (baseOrSpecial)
            return skillData.BaseAtkDt[indexCb].DamagePercent[indexDamage];
        else
            return skillData.SpecialAtkDt[indexCb].DamagePercent[indexDamage];
    }
    public float getDamagePercnet(AttackData attackData, int indexImpact)
    {
        return attackData.DamagePercent[indexImpact];
    }
    public AttackData GetAttackData_Attack(SkillData skillData, int cbCount, bool baseOrSpecial)
    {
        if (baseOrSpecial)
        {
            if (cbCount >= skillData.BaseAtkDt.Count)
            {
                cbCount = skillData.BaseAtkDt.Count - 1;
            }
            return skillData.BaseAtkDt[cbCount];
        }
        else
        {
            if (cbCount >= skillData.SpecialAtkDt.Count)
            {
                cbCount = skillData.SpecialAtkDt.Count - 1;
            }
            return skillData.SpecialAtkDt[cbCount];
        }
    }

}
