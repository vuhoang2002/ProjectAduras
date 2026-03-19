using UnityEngine;
using static cbValue;
public static class CombatProcessor
{
    public static int ProcessHit(Character attacker, Character defender, DamageInfo info, bool range = false)
    {
        CombatController cc = defender.GetComponent<CombatController>();
        bool isFront;
        if (range)
            isFront = IsAttackFromFront(defender.transform, info._impactPos);
        else
        {
            isFront = IsAttackFromFront(defender.transform, attacker.transform.position);

        }
        int finalDamage = CombatProcessor.CalculateFinalDamage(
            attacker,
            defender,
            info);
        if (cc.isGuard && isFront)
        {
            if (!range)
                ApllyRecoil(attacker);
            finalDamage = Mathf.RoundToInt(finalDamage * 1f);
            defender.GetComponent<Health>().SP.Consume(finalDamage);
            return 0;
        }
        return finalDamage;
    }
    public static int CalculateFinalDamage(
        Character attacker,
        Character defender,
        DamageInfo info)
    {
        int rawDamage = info._rawDamage;
        // 1️⃣ School bonus
        float schoolMultiplier = attacker.Stats.GetSchoolMultiplier(info._schoolType);
        float statMultiplier = CalculateStatScaling(attacker, info);
        Debug.Log("Rawdamage:" + rawDamage + " school:" + info._schoolType + " mul:" + schoolMultiplier + " stat:" + statMultiplier);

        rawDamage = Mathf.RoundToInt(rawDamage * (1f + schoolMultiplier + statMultiplier));

        // 2️⃣ Armor reduction
        int armor = 0;

        if (info._Type == DamageType.Physical)
            armor = defender.Stats.PhysicalArmor;
        else if (info._Type == DamageType.Magical)
            armor = defender.Stats.MagicArmor;

        int finalDamage = ApplyArmorFormula(rawDamage, armor);
        Debug.Log("armor: " + armor + " finaldame:" + finalDamage);
        return finalDamage;
    }

    private static int ApplyArmorFormula(int raw, int armor)
    {
        float reduced = raw * (100f / (100f + armor));
        return Mathf.RoundToInt(reduced);
    }
    private static float CalculateStatScaling(Character attacker, DamageInfo info)
    {
        float bonus = 0f;
        switch (info._statScale)
        {
            case StatPri.STR:
                bonus += attacker.Stats.STR.Value * 0.05f;
                break;
            case StatPri.DEX:
                bonus += attacker.Stats.DEX.Value * 0.05f;
                break;
            case StatPri.INT:
                bonus += attacker.Stats.INT.Value * 0.05f;
                break;

        }
        return bonus;
    }
    static bool IsAttackFromFront(Transform target, Vector3 attacker)
    {
        Vector3 dir = (attacker - target.position).normalized;

        float dot = Vector3.Dot(target.forward, dir);

        return dot > 0.3f;
    }
    private static void ApllyRecoil(Character attacker)
    {
        if (attacker.CompareTag(AdurasLayer.Enemy))
            attacker.GetComponent<StateMachine>()
            ._SwitchState(new EnemyRecoilState(attacker.GetComponent<StateMachine>() as EnemyStateMachine));
        else if (attacker.CompareTag(AdurasLayer.Player))
            attacker.GetComponent<StateMachine>()
            ._SwitchState(new PlayerRecoilState(attacker.GetComponent<StateMachine>() as PlayerStateMachine));

    }
}

