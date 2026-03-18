using UnityEngine;
using Unity.Cinemachine;
public class CameraControler : MonoBehaviour
{
    [SerializeField] private Vector3 offSet = new Vector3(-2, 0, 0);

    public CinemachineOrbitalFollow cinemachineOrbitalFollow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // _vc.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = offSet;
        // var orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        cinemachineOrbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        //cinemachineOrbitalFollow.TargetOffset = offSet;
        Cursor.visible = false;
    // Khóa chuột vào giữa màn hình
    Cursor.lockState = CursorLockMode.Locked;

    }
}


