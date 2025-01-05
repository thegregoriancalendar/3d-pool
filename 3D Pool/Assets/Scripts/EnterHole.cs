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
        if (other.gameObject.GetComponent<BallSelect>() != null)
        {
            other.gameObject.transform.position = StateHandler.scratchPlacement();
        } 
        else
        {
            StateHandler.ballsack.Remove(other.gameObject);
            Destroy(other.gameObject);
            StateHandler.score++;
            StateHandler.scoreText.GetComponent<TMP_Text>().text = "Score: " + StateHandler.score.ToString();
            GameObject.Find("ding").GetComponent<AudioSource>().Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
