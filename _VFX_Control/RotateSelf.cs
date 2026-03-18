using Unity.VisualScripting;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform parent;
    public float rotateSpeed = 90f;
    void Start()
    {
        transform.SetParent(parent, true);
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);

    }
}
