using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.Multiplayer.Center.NetcodeForGameObjectsExample
{
    /// <summary>
    /// A basic example of client authoritative movement. It works in both client-server and distributed-authority scenarios.
    /// If you want to modify this Script please copy it into your own project and add it to your Player Prefab.
    /// </summary>
    public class ClientAuthoritativeMovement : NetworkBehaviour
    {

        private float minX = 0;
        private float maxX = 0;
        private float minZ = 0;
        private float maxZ = 0;

        void Start()
        {
            // Retrieve the map object
            GameObject map = GameObject.FindGameObjectWithTag("Map");
            if (map != null) 
            {
                Renderer rend = map.GetComponent<Renderer>();
                Bounds bounds = rend.bounds;

                minX = bounds.min.x;
                maxX = bounds.max.x;

                minZ = bounds.min.z;
                maxZ = bounds.max.z;
            }
            else
            {
                Debug.LogWarning("Player Movement script could not find the map");
            }
        }
        /// <summary>
        /// Movement Speed
        /// </summary>
        public float Speed = 5;

        void Update()
        {
            // IsOwner will also work in a distributed-authoritative scenario as the owner 
            // has the Authority to update the object.
            if (!IsOwner || !IsSpawned) return;

            var multiplier = Speed * Time.deltaTime;

            // New input system backends are enabled.
            if (Keyboard.current.aKey.isPressed)
            {
                transform.position += new Vector3(-multiplier, 0, 0);
            }
            if (Keyboard.current.dKey.isPressed)
            {
                transform.position += new Vector3(multiplier, 0, 0);
            }
            if (Keyboard.current.wKey.isPressed)
            {
                transform.position += new Vector3(0, 0, multiplier);
            }
            if (Keyboard.current.sKey.isPressed)
            {
                transform.position += new Vector3(0, 0, -multiplier);
            }

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

            transform.position = pos;
        }
    }
}
