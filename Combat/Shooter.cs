using UnityEngine;

public class Shooter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected CombatController combatControler;
    public Shooter(CombatController combatControler)
    {
        this.combatControler = combatControler;
    }

}
