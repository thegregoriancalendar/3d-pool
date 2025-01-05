using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolume : MonoBehaviour
{
    public GameObject musicPlayer;

    void Start()
    {
        gameObject.GetComponent<Slider>().value = 0.5f;
        musicPlayer.GetComponent<AudioSource>().volume = gameObject.GetComponent<Slider>().value;
    }

    // Start is called before the first frame update
    public void ChangeVolume()
    {
        musicPlayer.GetComponent<AudioSource>().volume = gameObject.GetComponent<Slider>().value;
    }
}
