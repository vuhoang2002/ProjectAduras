using Unity.VisualScripting;
using UnityEngine;

public class EquipWeapon : AdurasMonobehavius
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerStateMachine _SM;
    Transform rightHand;
    Transform leftHand;
    public GameObject weapon;
    void Start()
    {
        rightHand =
            _SM.Animator.GetBoneTransform(HumanBodyBones.RightHand);
        leftHand =
            _SM.Animator.GetBoneTransform(HumanBodyBones.LeftHand);
        EquipWeaponOn(weapon);
    }
    public void EquipWeaponOn(GameObject weaponPrefab)
    {
        Transform rightHand =
            _SM.Animator.GetBoneTransform(HumanBodyBones.RightHand);

        GameObject weapon = Instantiate(weaponPrefab);

        weapon.transform.SetParent(rightHand);

        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;
    }
    public void AttachWeapon()
    {
        // weapon.transform.SetParent(rightHand);
    }

    public void DetachWeapon()
    {
        //  weapon.transform.SetParent(backSlot);
    }





}
