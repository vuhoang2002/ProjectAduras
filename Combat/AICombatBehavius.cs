using UnityEngine;
//hiển trong Behavius/AICombatBehavius
[CreateAssetMenu(fileName = "NewAICombatBehaviour", menuName = "Behaviours/AICombatBehaviour")]

public class AICombatBehaviour : ScriptableObject
{
    public int closeScore;
    public int rangerScore;
    public int defendScore;
    //more
    public float closeDistance;
    public float rangerDistance;
}
