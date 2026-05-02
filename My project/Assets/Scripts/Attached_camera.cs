using UnityEngine;

public class CameraAttach : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 10, 0);
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        cam.transform.position = cam.transform.position = transform.position + offset;
    }

    void Update()
    {

        cam.transform.position = transform.position + offset;
        cam.transform.LookAt(transform);
    }
}