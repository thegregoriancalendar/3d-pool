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

    private float botTimer;

    private Vector3 botDirection;


    void Start()
    {
        botTimer = 0f;

        StateHandler.ballRadius = ball.GetComponent<SphereCollider>().radius;
        StateHandler.scoreText = scoreText;
        StateHandler.predictionIndicator = predictionIndicator;

        // cue ball
        StateHandler.ballsack.Add(Instantiate(ball, new Vector3(-2, 1, 0), Quaternion.identity));

        // other balls
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                GameObject currentBall = Instantiate(ball, new Vector3(i, 1, j - i * 0.5f), Quaternion.identity);

                Destroy(currentBall.GetComponent<BallSelect>());
                currentBall.GetComponent<Renderer>().material.color = Color.HSVToRGB(Random.Range(0f,1f), 1f, 0.5f);

                StateHandler.ballsack.Add(currentBall);
            }
        }

        //pockets
        StateHandler.holes[0] = Instantiate(hole, new Vector3(8.75f, 1, 8.75f), Quaternion.identity);
        StateHandler.holes[1] = Instantiate(hole, new Vector3(-8.75f, 1, 8.75f), Quaternion.identity);
        StateHandler.holes[2] = Instantiate(hole, new Vector3(8.75f, 1, -8.75f), Quaternion.identity);
        StateHandler.holes[3] = Instantiate(hole, new Vector3(-8.75f, 1, -8.75f), Quaternion.identity);

    }

    public void updateThisOneBehaviorBecauseUnityIsAnnoying()
    {
        StateHandler.shootCue();
    }

    public void Update()
    {
        botTimer += Time.deltaTime;

        if (StateHandler.currentState == StateHandler.GameState.BALLS_MOVING)
        {
            if (StateHandler.getNetMovement() < StateHandler.movementThreshold)
            {
                StateHandler.currentState = StateHandler.GameState.SELECT_BALL;
                //StateHandler.player1Turn = !StateHandler.player1Turn;
                StateHandler.player1Turn = false;
                botTimer = 0f;
            }

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


        // bot logic

        if (!StateHandler.player1Turn && StateHandler.singlePlayer)
        {
            switch (StateHandler.currentState)
            {
                case StateHandler.GameState.SELECT_BALL:
                    if (botTimer < 1f) { break; }

                    if (StateHandler.hasScratched)
                    {
                        deScratch();
                    }

                    if (botTimer > 2f)
                    {
                        StateHandler.ballsack[0].GetComponent<BallSelect>().selectTheBall();
                        botDirection = StateHandler.directionToHit();
                    }

                    break;

                case StateHandler.GameState.ROTATE_CUE:
                    if (botTimer < 3f) { break; }

                    bool movementFinished = StateHandler.ballsack[0].GetComponent<BallSelect>().botMove(botDirection);

                    if (movementFinished && botTimer > 5f)
                    {
                        updateThisOneBehaviorBecauseUnityIsAnnoying(); //LMFAO
                    }
                    
                    break;
            }
        }
    }

    public void deScratch()
    {
        StateHandler.hasScratched = false;
        StateHandler.ballsack[0] = Instantiate(ball, StateHandler.scratchPlacement(), Quaternion.identity);
    }

}
