using UnityEngine;

using UnityEngine.UIElements;

using Unity.Multiplayer.Center.NetcodeForGameObjectsExample;

 

public class ResourcePanelController : MonoBehaviour

{
    public Ressources_collector Ressources_collector_instance;

    //private int bois = Resources_collector.compteur_bois;
    //private int pierre = Resources_collector.compteur_pierre;
    //private int fer = Resources_collector.compteur_fer;


    private Label m_WoodLabel;
    private Label m_StoneLabel;
    private Label m_IronLabel;

    private Label m_LifeLabel;
    private Label m_DamageLabel;
    private Label m_SpeedLabel;

    private bool playerFound = false;

    private PlayerShoot playerShootInstance;
    private ClientAuthoritativeMovement clientAuthoritativeMovementInstance;
    private PlayerManager playerManagerInstance;

    public void OnPrefabCreated(Ressources_collector instance)
    {
        Ressources_collector_instance = instance;

        //On va chercher la vie, les damages et la vitesse du bon joueur

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
        {
            Debug.LogWarning("Aucun objet Player trouvé dans la scène.");
            return;
        }

        foreach (GameObject player in players)
        {
            PlayerManager pm = player.GetComponent<PlayerManager>();
            if (pm != null && pm.isCurrentPlayer)
            {
                playerManagerInstance = pm;
                playerShootInstance = player.GetComponent<PlayerShoot>();
                clientAuthoritativeMovementInstance = player.GetComponent<ClientAuthoritativeMovement>();

                playerFound = true;
                RefreshUI();
                break;
            }
        }
    }

  

    void OnEnable()

    {

        var root = GetComponent<UIDocument>().rootVisualElement;

        m_WoodLabel  = root.Q<Label>("WoodLabel");

        m_StoneLabel = root.Q<Label>("StoneLabel");

        m_IronLabel  = root.Q<Label>("IronLabel");

        m_LifeLabel = root.Q<Label>("LifeLabel");

        m_DamageLabel = root.Q<Label>("DamageLabel");

        m_SpeedLabel = root.Q<Label>("SpeedLabel");

    }

 

    void Update()
    {
        if (!playerFound)
        {
            // Retry to find the player ... 
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach ( GameObject player in players)
            {
                PlayerManager pm = player.GetComponent<PlayerManager>();
                if (pm != null && pm.isCurrentPlayer)
                {
                    playerManagerInstance = pm;
                    playerShootInstance = player.GetComponent<PlayerShoot>();
                    clientAuthoritativeMovementInstance = player.GetComponent<ClientAuthoritativeMovement>();
                    playerFound = true;
                    break;
                }
            }
        }
        if (playerFound == true)
            RefreshUI();
        
    }

 

    private void RefreshUI()
    {

        if (m_WoodLabel != null)
            m_WoodLabel.text = Ressources_collector_instance.compteur_bois.ToString();

        if (m_IronLabel != null)
            m_IronLabel.text = Ressources_collector_instance.compteur_fer.ToString();

        if (m_StoneLabel != null)
            m_StoneLabel.text = Ressources_collector_instance.compteur_pierre.ToString();
        
        if (m_LifeLabel != null)
            m_LifeLabel.text = playerManagerInstance.life.Value.ToString() + " / " + playerManagerInstance.maxLife.ToString();
        
        if (m_DamageLabel != null)
            m_DamageLabel.text = playerShootInstance.damage.ToString();
        
        if (m_SpeedLabel != null)
            m_SpeedLabel.text = clientAuthoritativeMovementInstance.Speed.ToString();
    }


}

 