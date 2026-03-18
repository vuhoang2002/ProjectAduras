using UnityEditor.Analytics;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Collider[] allCollider;
    private Rigidbody[] allRigidBody;
    public CharacterController CharacterController;
    public Animator Animator;

    void Start()
    {
        allCollider = GetComponentsInChildren<Collider>(true);//Components có chữ S
        allRigidBody = GetComponentsInChildren<Rigidbody>(true);
        // _SMch = GetComponent<StateMachine>();
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        ToggleRagdoll(false);
    }
    public void ToggleRagdoll(bool isRagdoll)
    {
        foreach (var col in allCollider)
        {
            if (col.CompareTag(AdurasLayer.Ragdoll))
            {
                col.enabled = isRagdoll;

            }
        }

        foreach (var rb in allRigidBody)
        {
            if (rb.CompareTag(AdurasLayer.Ragdoll))
            {
                rb.isKinematic = !isRagdoll;
                rb.useGravity = isRagdoll;
            }
        }
        CharacterController.enabled = !isRagdoll;
        Animator.enabled = !isRagdoll;


    }
}