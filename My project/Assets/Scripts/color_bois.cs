using UnityEngine;

public class color_bois : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       GetComponent<Renderer>().material.color = Color.brown; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
