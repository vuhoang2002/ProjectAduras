using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterDefinition", menuName = "Stats/Character Definition")]
public class CharacterDefinition : ScriptableObject
{
    [Header("Primary Stats")]
    public int STR;
    public int DEX;
    public int INT;
    public int CON;
    public int END;
    public int WIS;

    [Header("School Levels")]
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
}