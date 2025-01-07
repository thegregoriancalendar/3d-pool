using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnterHole : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == StateHandler.ballsack[0])
        {
            StateHandler.hasScratched = true;
            StateHandler.ballsack[0] = null;
            //StateHandler.ballsack[0].transform.position = StateHandler.scratchPlacement();
        } 
        else
        {
            StateHandler.score++;
            StateHandler.scoreText.GetComponent<TMP_Text>().text = "Score: " + StateHandler.score.ToString();
            GameObject.Find("ding").GetComponent<AudioSource>().Play();
            StateHandler.ballsack.Remove(other.gameObject);
            //Destroy(other.gameObject);
        }
        
        Destroy(other.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
