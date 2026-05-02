using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerManager))]
public class PlayerShoot : NetworkBehaviour
{
    [Header("Settings")]
    public GameObject bouletPrefab;
    public float bouletSpeed = 15f;
    public float fireRate = 0.3f;
    public float spawnOffset = 1.5f;

    private float _lastFireTime;
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        // Only the local owner can fire
        if (!IsOwner) return;
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time - _lastFireTime >= fireRate)
        {
            _lastFireTime = Time.time;

            Vector3 direction = GetDirectionToMouse();
            Vector3 spawnPos = transform.position + direction*spawnOffset;

            FireServerRpc(spawnPos, direction);
        }
    }

    private Vector3 GetDirectionToMouse()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            Vector3 dir = (worldPoint - transform.position);
            dir.y = 0;
            return dir.normalized;
        }

        return transform.forward;
    }

    [ServerRpc]
    private void FireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        GameObject obj = Instantiate(bouletPrefab, spawnPos, Quaternion.LookRotation(direction));
        NetworkObject netObj = obj.GetComponent<NetworkObject>();
        netObj.Spawn();

        BouletManager boulet = obj.GetComponent<BouletManager>();
        boulet.Initialize(direction, bouletSpeed);
    }
}