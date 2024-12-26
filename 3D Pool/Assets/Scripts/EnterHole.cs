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
        StateHandler.ballsack.Remove(other.gameObject);
        Destroy(other.gameObject);
        StateHandler.score++;
        StateHandler.scoreText.GetComponent<TMP_Text>().text = "Score: " + StateHandler.score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
