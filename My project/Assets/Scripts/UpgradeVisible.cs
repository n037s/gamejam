using UnityEngine;
using System.Collections.Generic;



public class UpgradeVisible : MonoBehaviour
{

    [SerializeField] private List<SpriteRenderer> DefenseOverlayRenderer;
    [SerializeField] private List<SpriteRenderer> SeedOverlayRenderer;
    [SerializeField] private List<SpriteRenderer> AttackOverlayRenderer;

    void Start()
    {
        foreach (SpriteRenderer renderer in DefenseOverlayRenderer)
        {
            renderer.enabled = false;
        }
        foreach (SpriteRenderer renderer in SeedOverlayRenderer)
        {
            renderer.enabled = false;
        }
        foreach (SpriteRenderer renderer in AttackOverlayRenderer)
        {
            renderer.enabled = false;
        }     
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
  

    public void UpdateDefenseVisibility(int newDefenseLevel)
    
    {
        Debug.Log("la fonction UpdateDefenseVisibility a été appelée avec newDefenseLevel: " + newDefenseLevel);
        for (int i = 0; i < DefenseOverlayRenderer.Count; i++)
        {
            if (i != newDefenseLevel-1)
            {
                DefenseOverlayRenderer[i].enabled = false;
            }
            else
            {
                DefenseOverlayRenderer[i].enabled = true;
                Debug.Log("DefenseOverlayRenderer[" + i + "] enabled");
            }
        }
    }

    public void UpdateSpeedVisibility(int newSpeedLevel)
    
    {
        Debug.Log("la fonction UpdateSpeedVisibility a été appelée avec newSpeedLevel: " + newSpeedLevel);
        for (int i = 0; i < SeedOverlayRenderer.Count; i++)
        {
            if (i != newSpeedLevel-1)
            {
                SeedOverlayRenderer[i].enabled = false;
            }
            else
            {
                SeedOverlayRenderer[i].enabled = true;
                Debug.Log("SeedOverlayRenderer[" + i + "] enabled");
            }
        }
    }

    public void UpdateAttackVisibility(int newAttackLevel)
    
    {
        Debug.Log("la fonction UpdateAttackVisibility a été appelée avec newAttackLevel: " + newAttackLevel);
        for (int i = 0; i < AttackOverlayRenderer.Count; i++)
        {
            if (i != newAttackLevel-1)
            {
                AttackOverlayRenderer[i].enabled = false;
            }
            else
            {
                AttackOverlayRenderer[i].enabled = true;
                Debug.Log("AttackOverlayRenderer[" + i + "] enabled");
            }
        }
    }
}

