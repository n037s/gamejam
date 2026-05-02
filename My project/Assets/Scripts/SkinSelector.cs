using UnityEngine;
using System.Collections.Generic;

public class SkinSelector : MonoBehaviour
{
    public List<RuntimeAnimatorController> skins;
    
    public void Start()
    {
        int skin_number = PlayerPrefs.GetInt("skin_number", -1);

        if (skin_number == -1)
        {
            skin_number = UnityEngine.Random.Range(0, skins.Count);
        }

        GetComponent<Animator>().runtimeAnimatorController = skins[skin_number];
    }
}