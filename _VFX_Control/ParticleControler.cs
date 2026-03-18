using UnityEngine;

public class ParticleControler : MonoBehaviour
{
  [SerializeField] private ParticleSystem[] ps;
   
    public void PlayPS()
    {
        if(ps==null || ps.Length == 0) return;
        foreach (var item in ps)
        {
        item.Play();
        }
    }
}
