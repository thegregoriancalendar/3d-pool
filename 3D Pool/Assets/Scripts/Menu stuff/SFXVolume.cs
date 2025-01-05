using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SFXVolume : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().value = 0.5f;
        StateHandler.sfxvolume = gameObject.GetComponent<Slider>().value;
        GameObject.Find("boom").GetComponent<AudioSource>().volume = StateHandler.sfxvolume;
        GameObject.Find("click").GetComponent<AudioSource>().volume = StateHandler.sfxvolume;
        GameObject.Find("ding").GetComponent<AudioSource>().volume = StateHandler.sfxvolume;
    }

    public void ChangeVolume()
    {
        StateHandler.sfxvolume = gameObject.GetComponent<Slider>().value;
        GameObject.Find("boom").GetComponent<AudioSource>().volume = StateHandler.sfxvolume;
        GameObject.Find("click").GetComponent<AudioSource>().volume = StateHandler.sfxvolume;
        GameObject.Find("ding").GetComponent<AudioSource>().volume = StateHandler.sfxvolume;
    }
}
