using UnityEngine;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample;
using System.Collections.Generic;

public class Achats : MonoBehaviour
{

    //Prix du canon
    [SerializeField] private List<int> Prix_bois_canon;
    [SerializeField] private List<int> Prix_pierre_canon;
    [SerializeField] private List<int> Prix_fer_canon;

    [SerializeField] private List<int> Prix_bois_murailles;
    [SerializeField] private List<int> Prix_pierre_murailles;
    [SerializeField] private List<int> Prix_fer_murailles;

    [SerializeField] private List<int> Prix_bois_moteur;
    [SerializeField] private List<int> Prix_pierre_moteur;
    [SerializeField] private List<int> Prix_fer_moteur;
    //Prix des murailles
        //Améliorations

    [SerializeField] private List<int> Canon_damage_increase;
    [SerializeField] private List<int> Life_increase;
    [SerializeField] private List<int> Speed_increase;

    //Status d'amélioration
    public int canon_level = 0;
    public int murailles_level = 0; 
    public int moteur_level = 0;


    private Ressources_collector ressourcesCollectorInstance;

    private PlayerManager playerManagerInstance;
    private PlayerShoot playerShootInstance;
    private ClientAuthoritativeMovement clientAuthoritativeMovementInstance;


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
                playerManagerInstance = player.GetComponent<PlayerManager>();
                playerShootInstance = player.GetComponent<PlayerShoot>();
                clientAuthoritativeMovementInstance = player.GetComponent<ClientAuthoritativeMovement>();

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
        //Debug.Log("AchatCanon method called");
        if (ressourcesCollectorInstance == null)
        {
            Debug.LogWarning("AchatCanon appelé avant que le joueur courant soit initialisé.");
            return;
        }
        if (canon_level >= 3)
        {
            Debug.Log("Canon déjà au niveau maximum !");
            return;
        }
        //Debug.Log("le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);
           
        if (ressourcesCollectorInstance.compteur_bois >= Prix_bois_canon[canon_level] && ressourcesCollectorInstance.compteur_pierre >= Prix_pierre_canon[canon_level] && ressourcesCollectorInstance.compteur_fer >= Prix_fer_canon[canon_level])
        {


            ressourcesCollectorInstance.compteur_bois -= Prix_bois_canon[canon_level];
            ressourcesCollectorInstance.compteur_pierre -= Prix_pierre_canon[canon_level];
            ressourcesCollectorInstance.compteur_fer -= Prix_fer_canon[canon_level];

            Debug.Log("Achat du canon effectué, le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);

            //On améliore les dégats du canon et le niveau du canon
            playerShootInstance.damage += Canon_damage_increase[canon_level];
            canon_level += 1;

            //bouletManagerInstance.damage 
        
        }
        else
        {
            //Le joueur n'a pas assez de ressources pour acheter le canon
            Debug.Log("Vous n'avez pas assez de ressources pour acheter le canon !");
        }

    }

public void AchatMurailles()
    {
        //Debug.Log("AchatCanon method called");
        if (ressourcesCollectorInstance == null)
        {
            Debug.LogWarning("AchatMurailles appelé avant que le joueur courant soit initialisé.");
            return;
        }
        if (murailles_level >= 3)
        {
            Debug.Log("Murailles déjà au niveau maximum !");
            return;
        }
        //Debug.Log("le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);
           
        if (ressourcesCollectorInstance.compteur_bois >= Prix_bois_murailles[murailles_level] && ressourcesCollectorInstance.compteur_pierre >= Prix_pierre_murailles[murailles_level] && ressourcesCollectorInstance.compteur_fer >= Prix_fer_murailles[murailles_level])
        {


            ressourcesCollectorInstance.compteur_bois -= Prix_bois_murailles[murailles_level];
            ressourcesCollectorInstance.compteur_pierre -= Prix_pierre_murailles[murailles_level];
            ressourcesCollectorInstance.compteur_fer -= Prix_fer_murailles[murailles_level];

        

            //On améliore les dégats du canon et le niveau du canon
            playerManagerInstance.maxLife += Life_increase[murailles_level];
            murailles_level += 1;

            //bouletManagerInstance.damage 
        
        }
        else
        {
            //Le joueur n'a pas assez de ressources pour acheter le canon
            Debug.Log("Vous n'avez pas assez de ressources pour acheter le canon !");
        }

    }

public void AchatMoteur()
    {
        //Debug.Log("AchatCanon method called");
        if (ressourcesCollectorInstance == null)
        {
            Debug.LogWarning("AchatMoteur appelé avant que le joueur courant soit initialisé.");
            return;
        }
        if (moteur_level >= 3)
        {
            Debug.Log("Moteur déjà au niveau maximum !");
            return;
        }
        //Debug.Log("le compteur bois vaut : " + ressourcesCollectorInstance.compteur_bois + " le compteur pierre vaut : " + ressourcesCollectorInstance.compteur_pierre + " le compteur fer vaut : " + ressourcesCollectorInstance.compteur_fer);
           
        if (ressourcesCollectorInstance.compteur_bois >= Prix_bois_moteur[moteur_level] && ressourcesCollectorInstance.compteur_pierre >= Prix_pierre_moteur[moteur_level] && ressourcesCollectorInstance.compteur_fer >= Prix_fer_moteur[moteur_level])
        {


            ressourcesCollectorInstance.compteur_bois -= Prix_bois_moteur[moteur_level];
            ressourcesCollectorInstance.compteur_pierre -= Prix_pierre_moteur[moteur_level];
            ressourcesCollectorInstance.compteur_fer -= Prix_fer_moteur[moteur_level];

        

            //On améliore les dégats du canon et le niveau du canon
            clientAuthoritativeMovementInstance.Speed += Speed_increase[moteur_level];
            moteur_level += 1;

            //bouletManagerInstance.damage 
        
        }
        else
        {
            //Le joueur n'a pas assez de ressources pour acheter le canon
            Debug.Log("Vous n'avez pas assez de ressources pour acheter le canon !");
        }

    }
}