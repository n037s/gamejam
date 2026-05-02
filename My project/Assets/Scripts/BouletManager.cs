using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class BouletManager : NetworkBehaviour
{
    [Header("Settings")]
    public int damage = 10;
    public float lifetime = 10f; // despawn after this duration

    private Vector3 _direction;
    private float _speed;
    private float _spawnTime;

    public void Initialize(Vector3 direction, float speed)
    {
        _direction = direction.normalized;
        _speed = speed;
    }

    public override void OnNetworkSpawn()
    {
        _spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Server moves the bullet
        if (!IsServer) return;

        transform.position += _direction * _speed * Time.deltaTime;

        if (Time.time - _spawnTime >= lifetime)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            player.AddLife(-damage);
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
