using UnityEngine;

public class color_pierre : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.grey;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
