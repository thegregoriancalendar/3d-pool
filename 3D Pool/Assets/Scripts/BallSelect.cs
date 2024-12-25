using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSelect : MonoBehaviour
{
    public GameObject stick;
    public GameObject leftArrow;
    public GameObject rightArrow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (StateHandler.currentState == StateHandler.GameState.SELECT_BALL)
        {
            Instantiate(stick, transform);
            GameObject rightInstance = Instantiate(rightArrow, transform.position + new Vector3(0, 0, -2.5f), Quaternion.Euler(new Vector3(-90, 0, 180)));
            GameObject leftInstance = Instantiate(leftArrow, transform.position + new Vector3(0, 0, -2.5f), Quaternion.Euler(new Vector3(90, 180, 180)));
            ArrowDrag.queueBall = gameObject;
            rightInstance.GetComponent<ArrowDrag>().otherArrow = leftInstance;
            leftInstance.GetComponent<ArrowDrag>().otherArrow = rightInstance;
            StateHandler.currentState = StateHandler.GameState.ROTATE_CUE;
        }
    }
}
