using System;
using System.Collections.Generic;
using UnityEngine;
using static cbValue;
[CreateAssetMenu(fileName = "NewAttackData", menuName = "_ScriptableObjects/AttackData")]
public class AttackData : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Tooltip("Phân loại tấn công")]
    public AttackType AttackType;


    [Tooltip("Giá trị đại diện cho loại sát thương nguyên tố")]
    public SchoolType AttackElemental = SchoolType.None;

    [Tooltip("Loại sát thương")]
    public DamageType AttackDamageType = DamageType.Physical;

    [Tooltip("Xác định chiêu thức tốn sp, mp hay hp")]
    public SerializableDictionary<AttackCost, int> AttackCostTypes = new SerializableDictionary<AttackCost, int> { { AttackCost.Stamina, 0 }, { AttackCost.Magika, 0 }, { AttackCost.Health, 0 } };
    [Tooltip("Tên của anim khi thả đòn, với normal thì là đòn ở mang 0")]
    public List<string> AnimRelease = new List<string>(1);//thả đòn
    [Tooltip("Với chỉ số chẵn là impact time, lẻ là cooldowntime và phần tử sau phải lớn hơn phần tử trước đó")]
    public List<float> ImpactTime = new List<float>() { 0f, 1f };//turn on/off collider and apply damage
    [Range(0, 1)]
    [Tooltip("Giá trị này đại diện cho thời gian hoàn thành anim sau thời gian này combo sẽ bị reset")]
    public float OverTime = 1f;
    [Tooltip("Sát thương gây ra mỗi giây, áp dụng khi hold")]
    public float AttackPerTime = 0.3f;//for hold
    [Range(0, 2)]
    [Tooltip("Giá trị đại diện cho sát thương dựa trên vũ khí hoặc nguyên tố, số phần từ phải bằng 1 nửa impactime ")]
    public List<float> DamagePercent = new List<float>(1);//damage percentage
    [Tooltip("Đòn tấn công này có cho phép thi triển khi nhân vật đang di chuyển hay không")]
    public atkMoved IsCanMove = atkMoved.None;
    [Tooltip("Giá trị xác định xem khi 1 tay đang sử dụng, tay khác có thể kích hoạt đòn đánh hay không")]
    public AllowActionOtherHand AllowActionOtherHand = AllowActionOtherHand.None;
    //turn on if allow is true
    [Tooltip("Tag của những kỹ năng cho phép thi triển nếu AllowActionOtherHand được bật")]
    public List<String> allowOtherSkill;//tag
    [Tooltip("Can be null")]
    public String skillTag;
    public ComboStatus ComboStatus;
    public List<SkillDefinition> skillDefinition;
    public List<ForceData> forceData = new List<ForceData>();
    public EffectStatus EffectStatus;
    public StatPri statPri;
    public HitBoxZone hitBoxZone;
}
