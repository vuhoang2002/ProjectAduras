using System.Collections.Generic;
using UnityEngine;
using static cbValue;
[CreateAssetMenu(fileName = "NewSkillData", menuName = "_ScriptableObjects/SkillData")]

public class SkillData : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AttackProperty attackProperty;
    public AttackType attackType;
    public CbArm cbHand;
    public AnimatorOverrideController aoe;
    public List<float> chargeTime = new List<float>();//cái đầu tiên là tg bé nhất, cái cuối cùng là tg lớn nhất, sau tg này tự phát lực
    public List<AttackData> BaseAtkDt = new List<AttackData>();//danh cho cac don danh thuong
    public List<AttackData> SpecialAtkDt = new List<AttackData>();//danh cho cac don hold
    public List<string> AnimCharging = new List<string>(1);//thả đòn
    public List<string> AnimLoopCharge = new List<string>(1);//thả đòn
    public SkillDefinition effectSkillCharging;// hiệu ứng của đòn đánh khi niệm lực
    public bool ifinityCharge;


}
