using UnityEngine;

public static class cbValue
{
    public enum CbArm
    {
        Primary,
        Secondary
    }

    public enum AttackType
    {
        Normal,
        Charging,
        Charge_Normal,
        Accumulation,//<ifrit descend> tích tụ sức mạnh vào tay thi triển, chỉ cần chuột <ko cần q/e> lần nửa sẽ thi triển đòn được tích tụ sẵn ở tay
        Hold,//flame
        DefShield,//shield
        None
    }
    public enum AttackProperty
    {
        Melee,//turn on hixbox 
        Shooter,
        MeleeRanger,// đòn kiếm khí
        Shield,
    }
    public enum AttackPhase
    {
        None,
        WindUp,
        Impact,
        CoolDown,
        Over,
        Charging
    }
    public enum AttackElemental
    {
        Metal,
        Wood,
        Water,
        Fire,
        Earth,
        Air,
        Thunder,
        Poison,
        None
    }
    public enum SchoolType
    {
        None,
        Fire,
        Water,
        Lightning,
        Ice,
        Earth,
        Metal,
        Wood,
        Wind,
        Blood,
        Warfare,
        Assassin,
        Marksman,
        Holy,
        Necromancy,
        Poison
    }
    public enum DamageType
    {
        Physical,
        Magical,
        True,
    }
    public enum EffectStatus // hiệu ứng phép thuật
    {
        None,
        Stun,// choáng ( lờ đờ lảo do bị sét đánh)  
        Wet,
        Cold,
        Freeze,
        Warm,
        Burn,
        Poisoned,
        Electrified,
        Bleed,
        Healing,
    }
    public enum KnockOnEffect
    {
        None,
        MiniStun,
        KnockDown,
        KnockBack,
        HeadDown,
    }
    public enum atkMoved
    {
        None,
        Allow,
        OnlyWhenCompleteCharging,
        Slow
    }
    public enum AllowActionOtherHand
    {
        None,
        Allow,
    }

    public enum CbSkill
    {
        None,
        Qskill,
        Eskill,
        Q_Eskill
    }
    public static void Printf(string message)
    {
        Debug.Log(message);
    }
    public enum ComboStatus
    {
        Add,
        NoAdd,
        Reset
    }
    public enum AttackCost
    {
        Stamina,
        Magika,
        Health
    }
    public enum DoubleJump
    {
        None,
        Action,
        Done // đã nhảy
    }
    public struct DamageInfo
    {
        public int _rawDamage;
        public float _damagePercent;
        public Vector3 _impactPos;
        public SchoolType _schoolType;
        public DamageType _Type;
        public EffectStatus _effectStatus;
        public KnockOnEffect _knockOnEffect;
        public bool isCrit;
        public Vector3 _force;
        public StatPri _statScale;
    }
    public enum StatPri
    {
        STR,
        DEX,
        INT,
        CON,
        END,
        WIS
    }
    public enum SpawnEffectPos
    {
        RightArm,
        LeftArm,
        Feet,
        RightWeapon,
        LeftWeapon,
        CenterBody
    }
    public enum HitBoxZone
    {
        RightWeapon,
        LeftWeapon,
        RightArm,
        LeftArm,
        RightLeg,
        LeftLeg
    }

}
