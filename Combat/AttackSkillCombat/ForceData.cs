using UnityEngine;
using static cbValue;
[CreateAssetMenu(fileName = "ForceData", menuName = "ForceData")]
public class ForceData : ScriptableObject
{
    public Vector3 forceDirection;
    public KnockOnEffect knockOnEffect;

}
