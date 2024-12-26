using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpdateThisOneBehaviorBecauseUnityIsAnnoying : MonoBehaviour
{
    public GameObject poolTable;
    public GameObject predictionIndicator;
    public GameObject ball;
    public GameObject hole;
    public GameObject scoreText;

    private StateHandler.GameState prevState;

    void Start()
    {
        StateHandler.scoreText = scoreText;
        StateHandler.predictionIndicator = predictionIndicator;

        // cue ball
        StateHandler.ballsack.Add(Instantiate(ball, new Vector3(-2, 1, 0), Quaternion.identity));

        // other balls
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                StateHandler.ballsack.Add(Instantiate(ball, new Vector3(i, 1, j - i*0.5f), Quaternion.identity));
            }
        }

        //pockets
        Instantiate(hole, new Vector3(8.75f, 1, 8.75f), Quaternion.identity);
        Instantiate(hole, new Vector3(-8.75f, 1, 8.75f), Quaternion.identity);
        Instantiate(hole, new Vector3(8.75f, 1, -8.75f), Quaternion.identity);
        Instantiate(hole, new Vector3(-8.75f, 1, -8.75f), Quaternion.identity);

    }

    public void updateThisOneBehaviorBecauseUnityIsAnnoying()
    {
        StateHandler.shootCue();
    }
    public void Update()
    {
        if (StateHandler.currentState == StateHandler.GameState.BALLS_MOVING)
        {
            if (StateHandler.getNetMovement() < StateHandler.movementThreshold)
            {
                StateHandler.currentState = StateHandler.GameState.SELECT_BALL;
                StateHandler.player1Turn = !StateHandler.player1Turn;
            }

            Debug.Log(StateHandler.getNetMovement());
        }

        if (StateHandler.currentState == StateHandler.GameState.SELECT_BALL) {
            StateHandler.haltBallMovement();
        }


        // displays changes in gamestate
        if (prevState != StateHandler.currentState)
        {
            Debug.Log(StateHandler.currentState);
        }

        prevState = StateHandler.currentState;
    }
}
