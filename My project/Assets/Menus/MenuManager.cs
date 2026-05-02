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
            foreach (GameObject menu in menus)
            {
                menu.SetActive(false);
            }

            // If we arrived here after a game ended, go straight to the socreboard
            if (ScoreboardData.HasResults)
                setDisplayedMenu(3);
            else
                setDisplayedMenu(1);
        }

        public void setDisplayedMenu(int menuID)
        {
            Debug.Log("Setting new menu : " + (menuID-1).ToString());
            
            if (displayedMenu != null)
            {
                displayedMenu.SetActive(false);
            }

            if (menuID-1 < menus.Count) 
            {
                displayedMenu = menus[menuID-1];
                displayedMenu.SetActive(true);
            }
        }
    }
}
