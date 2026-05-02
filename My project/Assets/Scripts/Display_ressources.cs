using UnityEngine;

using UnityEngine.UIElements;

 

public class ResourcePanelController : MonoBehaviour

{
    public Ressources_collector Ressources_collector_instance;

    //private int bois = Resources_collector.compteur_bois;
    //private int pierre = Resources_collector.compteur_pierre;
    //private int fer = Resources_collector.compteur_fer;


    private Label m_WoodLabel;
    private Label m_StoneLabel;
    private Label m_IronLabel;

    private bool playerFound = false;

    public void OnPrefabCreated(Ressources_collector instance)
    {
        Ressources_collector_instance = instance;
        
        playerFound = true;
        
        RefreshUI();
    }

    void OnEnable()

    {

        var root = GetComponent<UIDocument>().rootVisualElement;

        m_WoodLabel  = root.Q<Label>("WoodLabel");

        m_StoneLabel = root.Q<Label>("StoneLabel");

        m_IronLabel  = root.Q<Label>("IronLabel");



    }

 

    void Update()

    {
        
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
    }


}

 