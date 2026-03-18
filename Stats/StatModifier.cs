using System;

[System.Serializable]
public class StatModifier
{
    public int Flat;
    public float Percent;
    public object Source;

    public StatModifier(int flat, float percent, object source)
    {
        Flat = flat;
        Percent = percent;
        Source = source;
    }
}