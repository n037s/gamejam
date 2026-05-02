using UnityEngine;

public class CallOthersOnCreated : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //On s'assure qu'il s'agit du bon joueur
        PlayerManager playerManagerRef = GetComponent<PlayerManager>();
        
        if (playerManagerRef.isCurrentPlayer)
        {
            //On va signaler au Display_ressources que le prefab a été créé
            //On va signaler à Achats que le prefab a été crée
           
            
            // This method can be called from Ressources_collector to initialize the UI when the prefab is created.

             GameObject shopPanel = GameObject.FindWithTag("ShopPanel");
            if (shopPanel != null)
            {
                Achats AchatsInstance = shopPanel.GetComponent<Achats>();
                if (AchatsInstance == null)
                {
                    Debug.LogError("Achats component not found on the ShopPanel object.");
                }
                else
                {
                    AchatsInstance.onPrefabCreated(this);
                    Debug.Log("OnPrefabCreated method called");
                }
            }
            else
            {
                Debug.LogError("ShopPanel object with tag 'ShopPanel' not found in the scene.");
            }
        }   


    }
}