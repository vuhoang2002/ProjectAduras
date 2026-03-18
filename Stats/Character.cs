using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public CharacterStats Stats;
    [SerializeField] private CharacterDefinition characterDefinition;
    public bool isSpeedBuff { get; set; } = false;

    void Awake()
    {
        Stats = new CharacterStats(characterDefinition);
    }
}


