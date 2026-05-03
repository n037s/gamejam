using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;


[RequireComponent(typeof(PlayerManager))]
public class PlayerShoot : NetworkBehaviour
{
    [Header("Settings")]
    public GameObject bouletPrefab;
    public float bouletSpeed = 15f;
    public float fireRate = 0.3f;
    public float spawnOffset = 2f;
    public int yOffset = 2;

    public int damage = 10;

    private float _lastFireTime;
    private Camera _camera;
    public bool isdead = false;

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
            Vector3 spawnPos = transform.position + direction*spawnOffset + new Vector3(0, yOffset, 0);
            Debug.Log(IsMouseOverButtons());
            if (IsMouseOverButtons())
            {
                return; // Ne pas tirer si la souris est sur un bouton
            }
            if (isdead)
            {
                return; // Ne pas tirer si le joueur est mort
            }
            FireServerRpc(spawnPos, direction, damage);
        }
    }

    private bool IsMouseOverButtons()
    {
        GameObject buttonCanon = GameObject.FindWithTag("Canon");
        GameObject buttonMurailles = GameObject.FindWithTag("Murailles");
        GameObject buttonMoteur = GameObject.FindWithTag("Moteur");

        if (buttonCanon == null || buttonMurailles == null || buttonMoteur == null)
        {
            Debug.LogWarning("One or more buttons not found in the scene.");
            return false;
        }
         // Vérifie si la souris est sur un élément button
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Vérifie si l'objet sous la souris est ton bouton
            if(EventSystem.current.currentSelectedGameObject == buttonCanon){
                return true;
            }
            if(EventSystem.current.currentSelectedGameObject == buttonMurailles){
                return true;
            }
            if(EventSystem.current.currentSelectedGameObject == buttonMoteur){
                return true;
            }
        }

        return false;
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
    private void FireServerRpc(Vector3 spawnPos, Vector3 direction, int damage)
    {
        GameObject obj = Instantiate(bouletPrefab, spawnPos, Quaternion.LookRotation(direction));
        NetworkObject netObj = obj.GetComponent<NetworkObject>();
        netObj.Spawn();

        BouletManager boulet = obj.GetComponent<BouletManager>();
        boulet.Initialize(direction, bouletSpeed, damage, this.gameObject);
    }
}