using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine.UI;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (StateHandler.paused)
            {
                Resume();
            } 
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        StateHandler.paused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        StateHandler.paused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Application.Quit();
    }

}
