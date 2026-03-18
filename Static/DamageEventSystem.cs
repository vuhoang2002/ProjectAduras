using System;
using static cbValue;
public static class DamageEventSystem
{
    public static Action<int, DamageInfo> OnDamage;

    public static void Raise(int finalDamage, DamageInfo info)
    {
        OnDamage?.Invoke(finalDamage, info);
    }
}