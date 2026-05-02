using UnityEngine;

public class Ressources_collector : MonoBehaviour
{
    public GameObject bois;
    public int compteur_bois;
    public GameObject fer;
    public int compteur_fer;
    public GameObject pierre;
    public int compteur_pierre;

    private ResourcePanelController resourcePanelController_instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        compteur_bois = 0;
        compteur_fer = 0;
        compteur_pierre = 0;

        //On va signaler au Display_ressources que le prefab a été créé
        
        Debug.Log("OnPrefabCreated method called in ResourcePanelController.");
        // This method can be called from Ressources_collector to initialize the UI when the prefab is created.

        GameObject Panneau = GameObject.FindWithTag("Panneau ressources");
        if (Panneau != null)
        {
            resourcePanelController_instance = Panneau.GetComponent<ResourcePanelController>();
            if (resourcePanelController_instance == null)
            {
                Debug.LogError("ResourcePanelController component not found on the Panneau object.");
            }
            else
            {
                resourcePanelController_instance.OnPrefabCreated();
                Debug.Log("OnPrefabCreated method called");
            }
        }
        else
        {
            Debug.LogError("Panneau object with tag 'Panneau ressources' not found in the scene.");
        }
        

    }

    // Update is called once per frame
    void OnTriggerEnter(Collider collision)
    //private void OnCollisionEnter(Collision collision)
    {
        GameObject objetTouche = collision.gameObject;
        //Debug.Log("Collision détectée");

        if (objetTouche.CompareTag("Bois"))
        {
            compteur_bois++;
            Destroy(objetTouche);
            //Debug.Log("Bois collecté. Total bois: " + compteur_bois);
 
        }
        else if (objetTouche.CompareTag("Fer"))
        {
            compteur_fer++;
            Destroy(objetTouche);
        }
        else if (objetTouche.CompareTag("Pierre"))
        {
            compteur_pierre++;
            Destroy(objetTouche);
        }
    }
}
