using UnityEngine;

public class AudioPlayer : MonoBehaviour 
{
    private bool isOn = false;

    public void Start()
    {
        if (!isOn)
        {
            GetComponent<AudioSource>().Stop();
            isOn = true;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();

        }

    }
}