using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;

    private List<StatModifier> modifiers = new List<StatModifier>();

    private bool isDirty = true;
    private int lastValue;

    public int BaseValue => baseValue;

    public int Value
    {
        get
        {
            if (isDirty)
            {
                Recalculate();
            }
            return lastValue;
        }
    }

    public Stat(int baseValue)
    {
        this.baseValue = baseValue;
        isDirty = true;
    }

    public void SetBaseValue(int value)
    {
        baseValue = value;
        isDirty = true;
    }

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        isDirty = true;
    }

    public void RemoveModifier(StatModifier modifier)
    {
        if (modifiers.Remove(modifier))
        {
            isDirty = true;
        }
    }

    public void RemoveAllFromSource(object source)
    {
        if (modifiers.RemoveAll(m => m.Source == source) > 0)
        {
            isDirty = true;
        }
    }

    public void ClearAllModifiers()
    {
        modifiers.Clear();
        isDirty = true;
    }

    private void Recalculate()
    {
        int finalValue = baseValue;
        float percentBonus = 0f;

        foreach (var mod in modifiers)
        {
            finalValue += mod.Flat;
            percentBonus += mod.Percent;
        }

        finalValue = Mathf.RoundToInt(finalValue * (1 + percentBonus));

        lastValue = Mathf.Max(0, finalValue);
        isDirty = false;
    }
}