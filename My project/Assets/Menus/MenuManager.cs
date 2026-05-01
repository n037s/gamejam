using UnityEngine;
using UnityEngine.UIElements;

using System;
using System.Collections.Generic;

namespace menu
{
    public class MenuManager : MonoBehaviour
    {

        [SerializeField] public List<GameObject> menus = null;
        
        private GameObject displayedMenu = null;

        void Start()
        {
            setDisplayedMenu(0);
        }

        void public setDisplayedMenu(int menuID)
        {
            Debug.Log("Setting new menu : " + menuID.ToString());
            
            if (displayedMenu != null)
            {
                displayedMenu.SetActive(false);
            }

            if (menuID < menus.Count - 1) 
            {
                displayedMenu = menus[menuID];
                displayedMenu.SetActive(true);
            }
        }
    }
}
