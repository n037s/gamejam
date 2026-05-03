using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class SkinSelector : NetworkBehaviour
{
    public List<RuntimeAnimatorController> skins;
    
    public NetworkVariable<int> skinIndex = new NetworkVariable<int>(
        -1,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        skinIndex.OnValueChanged += OnSkinChanged;

        if (IsOwner)
        {
            int localSkin = PlayerPrefs.GetInt("skin_number", -1);
            if (localSkin < 0 || localSkin >= skins.Count)
                localSkin = UnityEngine.Random.Range(0, skins.Count);

            RequestSetSkinServerRpc(localSkin);
        }
        else if (skinIndex.Value >= 0)
        {
            ApplySkin(skinIndex.Value);
        }
    }

    [ServerRpc]
    private void RequestSetSkinServerRpc(int index)
    {
        skinIndex.Value = Mathf.Clamp(index, 0, skins.Count - 1);
    }

    private void OnSkinChanged(int oldValue, int newValue)
    {
        ApplySkin(newValue);
    }

    private void ApplySkin(int index)
    {
        if (index >= 0 && index < skins.Count)
        {
            GetComponent<Animator>().runtimeAnimatorController = skins[index];

            // Grece and Incas
            if (index == 0 || index == 3)
            {
                this.transform.localPosition = new Vector3(0, 0.3f, 0);
            }

            // Tolosa
            if (index == 2)
            {
                this.transform.localPosition = new Vector3(0, -0.9f, 0);
            }

            // Kulture
            if (index == 1)
            {
                this.transform.localPosition = new Vector3(0, -0.25f, 0);
            }
        }
    }
}