using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class StateHandler
{
    public static GameObject[] arrows = new GameObject[4];
    public static float pathSpacing = 0.1f;

    // score stuff

    public static int score = 0;
    public static GameObject scoreText;

    public static float movementThreshold = 1f; // total movement threshold below which balls will halt and gamestate will progress

    public static List<GameObject> ballsack = new List<GameObject>(); // list of all balls in scene

    public static GameObject[] holes = new GameObject[4];

    // gamestate stuff

    public enum GameState
    {
        SELECT_BALL,
        ROTATE_CUE,
        SHOOT_CUE,
        CUE_RELEASED,
        BALLS_MOVING
    }

    public static bool player1Turn = true;
    public static bool singlePlayer = false;
    public static GameState currentState = GameState.SELECT_BALL;

    // ball path planner 

    public static Ray[] paths = new Ray[2];
    public static float[] lengths = new float[2];

    public static GameObject predictionIndicator;
    private static List<GameObject> dingleList = new List<GameObject>();

    // UTILITY METHODS

    public static void shootCue()
    {
        currentState = GameState.SHOOT_CUE;
        foreach (GameObject arrow in arrows)
        {
            Object.Destroy(arrow);
        }
        arrows = new GameObject[4];

        foreach (GameObject dingle in dingleList)
        {
            Object.Destroy(dingle);
        }
        dingleList = new List<GameObject>();
    }

    public static void addArrow(GameObject arrow)
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (arrows[i] == null)
            {
                arrows[i] = arrow;
                return;
            }
        }
    }

    // ball path planner

    public static void updatePaths()
    {
        LayerMask layerMask = LayerMask.GetMask("Default", "control", "UI");
        for (int i = 0; i < paths.Length - 1; i++) {
            RaycastHit hit;
            if (Physics.Raycast(paths[i], out hit, layerMask))
            {
                lengths[i] = hit.distance;
                paths[i + 1].origin = hit.point;
                paths[i + 1].direction = paths[i].direction + hit.normal * 2;
            }
        }
    }

    public static void drawPaths()
    {
        foreach (GameObject dingle in dingleList)
        {
            Object.Destroy(dingle);
        }
        dingleList = new List<GameObject>();

        int currentRayNum = 0;
        float currentLength = 0;
        while (currentRayNum < paths.Length)
        {
            if (currentLength < lengths[currentRayNum])
            {
                GameObject dingle = Object.Instantiate(predictionIndicator, paths[currentRayNum].GetPoint(currentLength), Quaternion.identity);
                dingle.GetComponent<Renderer>().material.color = new Color(1, 1, 0, (lengths[currentRayNum] - currentLength) / lengths.Sum());
                dingleList.Add(dingle);
            }
            currentLength += pathSpacing;
            if (currentLength > lengths[currentRayNum])
            {
                currentLength = pathSpacing - (lengths[currentRayNum] - currentLength);
                currentRayNum++;
            }
        }
        
    }

    // cycling gamestate for player to cue

    public static float getNetMovement()
    {
        float totalVelocity = 0;
        foreach (GameObject ball in ballsack)
        {
            totalVelocity += ball.GetComponent<Rigidbody>().velocity.sqrMagnitude;
        }
        return totalVelocity;
    }

    public static void haltBallMovement()
    {
        foreach (GameObject ball in ballsack)
        {
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().rotation = Quaternion.identity;
        }
    }

    // ai behavior

    public static float directionToHit()
    {
        List<GameObject> candidates = new List<GameObject>();
        foreach (GameObject hole in holes)
        {
            foreach (GameObject ball in ballsack)
            {
                if (ball.GetComponent<BallSelect>() == null)
                {
                    Ray ray = new Ray(ball.transform.position, hole.transform.position - ball.transform.position);
                    if (Physics.Raycast(ray, Mathf.Infinity, 7))
                    {
                        continue;
                    }

                    candidates.Add(ball);
                }
            }
        }

        // if no valid hit can be found, perform random hit
        return Random.Range(0, 2 * Mathf.PI);
    }
}
