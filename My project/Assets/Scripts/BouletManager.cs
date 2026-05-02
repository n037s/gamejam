using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class BouletManager : NetworkBehaviour
{
    [Header("Settings")]
    public float lifetime = 10f; // despawn after this duration

    private int _damage = 10;
    private Vector3 _direction;
    private float _speed;
    private float _spawnTime;
    private GameObject _Firerer;

    public void Initialize(Vector3 direction, float speed, int damage, GameObject Firerer)
    {
        _direction = direction.normalized;
        _speed = speed;
        _damage = damage;
        _Firerer = Firerer;
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
        if (player != null && player.gameObject != _Firerer)
        {
            player.TakeDamage(_damage, _Firerer);
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
