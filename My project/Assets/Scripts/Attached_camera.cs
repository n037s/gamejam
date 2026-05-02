using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class CameraAttach : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 10, 0);
    private Camera cam;

    private PlayerManager playerManagerRef = null;

    void Start()
    {
        playerManagerRef = GetComponent<PlayerManager>();
    }

    void Update()
    {
        if (playerManagerRef && playerManagerRef.isCurrentPlayer)
        {
            if (!cam)
            {
                cam = Camera.main;
            }

            cam.transform.position = transform.position + offset;
            cam.transform.LookAt(transform);
        }
    }
}