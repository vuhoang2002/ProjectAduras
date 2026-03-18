using UnityEngine;
using System;
using static cbValue;
[System.Serializable]
public class CharacterStats
{
    /* ==============================
     * PRIMARY STATS
     * ==============================*/

    [Header("Primary Stats")]
    public Stat STR;
    public Stat DEX;
    public Stat INT;
    public Stat CON;
    public Stat END;
    public Stat WIS;

    /* ==============================
     * SCHOOL LEVELS
     * ==============================*/

    [Header("Schools")]
    public int Fire;
    public int Water;
    public int Lightning;
    public int Ice;
    public int Earth;
    public int Metal;
    public int Wood;
    public int Wind;
    public int Blood;
    public int Warfare;
    public int Assassin;
    public int Marksman;
    public int Holy;
    public int Necromancy;
    public int Poison;

    /* ==============================
     * RUNTIME
     * ==============================*/

    [Header("Runtime")]
    public int CurrentHP;
    public int CurrentStamina;
    public int CurrentMana;

    /* ==============================
     * DERIVED STATS
     * ==============================*/

    #region Derived

    public int MaxHP => CON.Value * 10;

    public int MaxStamina => END.Value * 10;

    public int MaxMana => WIS.Value * 10;

    public int PhysicalArmor => CON.Value * 3;

    public int MagicArmor => WIS.Value * 3;

    public int MeleeDamage => STR.Value * 2;

    public int RangedDamage => DEX.Value * 2;

    public int MagicDamage => INT.Value * 2;


    #endregion

    /* ==============================
     * SCHOOL BONUS
     * ==============================*/

    public float GetSchoolMultiplier(SchoolType school)
    {
        int level = GetSchoolLevel(school);
        return level * 0.05f;
    }

    public int GetSchoolLevel(SchoolType school)
    {
        switch (school)
        {
            case SchoolType.Fire: return Fire;
            case SchoolType.Water: return Water;
            case SchoolType.Lightning: return Lightning;
            case SchoolType.Ice: return Ice;
            case SchoolType.Earth: return Earth;
            case SchoolType.Metal: return Metal;
            case SchoolType.Wood: return Wood;
            case SchoolType.Wind: return Wind;
            case SchoolType.Blood: return Blood;
            case SchoolType.Warfare: return Warfare;
            case SchoolType.Assassin: return Assassin;
            case SchoolType.Marksman: return Marksman;
            case SchoolType.Holy: return Holy;
            case SchoolType.Necromancy: return Necromancy;
            case SchoolType.Poison: return Poison;
            default: return 0;
        }
    }

    /* ==============================
     * INIT
     * ==============================*/

    public void Init()
    {
        CurrentHP = MaxHP;
        CurrentStamina = MaxStamina;
        CurrentMana = MaxMana;
    }
    public CharacterStats(CharacterDefinition def)
    {
        STR = new Stat(def.STR);
        DEX = new Stat(def.DEX);
        INT = new Stat(def.INT);
        CON = new Stat(def.CON);
        END = new Stat(def.END);
        WIS = new Stat(def.WIS);

        Fire = def.Fire;
        Water = def.Water;
        Lightning = def.Lightning;
        Ice = def.Ice;
        Earth = def.Earth;
        Metal = def.Metal;
        Wood = def.Wood;
        Wind = def.Wind;
        Blood = def.Blood;
        Warfare = def.Warfare;
        Assassin = def.Assassin;
        Marksman = def.Marksman;
        Holy = def.Holy;
        Necromancy = def.Necromancy;
        Poison = def.Poison;

        Init();
    }
}