using UnityEngine;

public class Achats : MonoBehaviour
{

    //Prix du canon
    public int prix_bois_canon;
    public int prix_pierre_canon;
    public int prix_fer_canon;

    //Prix des murailles
    public int prix_bois_murailles;
    public int prix_pierre_murailles;
    public int prix_fer_murailles;

        //Prix du moteur
    public int prix_bois_moteur;
    public int prix_pierre_moteur;
    public int prix_fer_moteur;


    private Ressources_collector ressourcesCollectorInstance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void onPrefabCreated(CallOthersOnCreated instance)
    {
        Debug.Log("Achats.onPrefabCreated called");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
        {
            Debug.LogWarning("Aucun objet Player trouvé dans la scène.");
            return;
        }

        foreach (GameObject player in players)
        {
            PlayerManager playerManagerRef = player.GetComponent<PlayerManager>();
            if (playerManagerRef.isCurrentPlayer)
            {
        
                ressourcesCollectorInstance = player.GetComponent<Ressources_collector>();
                Debug.Log("Ressources_collector created");
                if (ressourcesCollectorInstance == null)
                {
                    Debug.LogError("Ressources_collector component not found on the Player object.");
                }

            }
        } 
    }


    // Méthode appelée par le bouton onClick
    public void AchatCanon()
    {
        Debug.Log("AchatCanon method called");
        if (ressourcesCollectorInstance == null)
        {
            Debug.LogWarning("AchatCanon appelé avant que le joueur courant soit initialisé.");
            return;
        }
        Debug.Log("le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);
           
        if (ressourcesCollectorInstance.compteur_bois >= prix_bois_canon && ressourcesCollectorInstance.compteur_pierre >= prix_pierre_canon && ressourcesCollectorInstance.compteur_fer >= prix_fer_canon)
        {
            //Le joueur a assez de ressources pour acheter le canon
            //On va soustraire les ressources du joueur
            Debug.Log("Achat du canon possible, le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);
            ressourcesCollectorInstance.compteur_bois -= prix_bois_canon;
            ressourcesCollectorInstance.compteur_pierre -= prix_pierre_canon;
            ressourcesCollectorInstance.compteur_fer -= prix_fer_canon;

            Debug.Log("Achat du canon effectué, le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);

            //On va instancier le canon à la position du joueur
            //GameObject canon = Instantiate(canonPrefab, transform.position, Quaternion.identity);
            //On peut aussi ajouter une logique pour faire en sorte que le canon soit placé à une position spécifique par rapport au joueur, ou pour faire en sorte que le canon suive le joueur, etc.
        }
        else
        {
            //Le joueur n'a pas assez de ressources pour acheter le canon
            Debug.Log("Vous n'avez pas assez de ressources pour acheter le canon !");
        }

    }

public void AchatMurailles()
    {
        Debug.Log("AchatMurailles method called");
        if (ressourcesCollectorInstance == null)
        {
            Debug.LogWarning("AchatMurailles appelé avant que le joueur courant soit initialisé.");
            return;
        }
        Debug.Log("le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);
           
        if (ressourcesCollectorInstance.compteur_bois >= prix_bois_murailles && ressourcesCollectorInstance.compteur_pierre >= prix_pierre_murailles && ressourcesCollectorInstance.compteur_fer >= prix_fer_murailles)
        {
            //Le joueur a assez de ressources pour acheter les murailles
            //On va soustraire les ressources du joueur
            Debug.Log("Achat des murailles possible, le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);
            ressourcesCollectorInstance.compteur_bois -= prix_bois_murailles;
            ressourcesCollectorInstance.compteur_pierre -= prix_pierre_murailles;
            ressourcesCollectorInstance.compteur_fer -= prix_fer_murailles;

            Debug.Log("Achat des murailles effectué, le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);

                //On va instancier les murailles à la position du joueur
                //GameObject murailles = Instantiate(muraillesPrefab, transform.position, Quaternion.identity);
                //On peut aussi ajouter une logique pour faire en sorte que le murailles soit placé à une position spécifique par rapport au joueur, ou pour faire en sorte que le murailles suive le joueur, etc.
            }
        else
        {
            //Le joueur n'a pas assez de ressources pour acheter les murailles
            Debug.Log("Vous n'avez pas assez de ressources pour acheter les murailles !");
        }

    }

public void AchatMoteur()
    {
        Debug.Log("AchatMoteur method called");
        if (ressourcesCollectorInstance == null)
        {
            Debug.LogWarning("AchatMoteur appelé avant que le joueur courant soit initialisé.");
            return;
        }
        Debug.Log("le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);
           
        if (ressourcesCollectorInstance.compteur_bois >= prix_bois_moteur && ressourcesCollectorInstance.compteur_pierre >= prix_pierre_moteur && ressourcesCollectorInstance.compteur_fer >= prix_fer_moteur)
        {
            //Le joueur a assez de ressources pour acheter les moteurs
            //On va soustraire les ressources du joueur
            Debug.Log("Achat des moteurs possible, le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);
            ressourcesCollectorInstance.compteur_bois -= prix_bois_moteur;
            ressourcesCollectorInstance.compteur_pierre -= prix_pierre_moteur;
            ressourcesCollectorInstance.compteur_fer -= prix_fer_moteur;

            Debug.Log("Achat des moteurs effectué, le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);

                //On va instancier les moteurs à la position du joueur
                //GameObject moteurs = Instantiate(moteursPrefab, transform.position, Quaternion.identity);
                //On peut aussi ajouter une logique pour faire en sorte que le moteurs soit placé à une position spécifique par rapport au joueur, ou pour faire en sorte que le moteurs suive le joueur, etc.
            }
        else
        {
            //Le joueur n'a pas assez de ressources pour acheter les moteurs
            Debug.Log("Vous n'avez pas assez de ressources pour acheter les moteurs !");
        }

    }
    
}
