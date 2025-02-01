using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class StateHandler
{
    public static GameObject[] arrows = new GameObject[4];
    public static float pathSpacing = 0.1f;

    // score stuff

    public static int score = 0;
    public static GameObject scoreText;

    public static float movementThreshold = 1f; // total movement threshold below which balls will halt and gamestate will progress

    public static List<GameObject> xiJinping = new List<GameObject>();

    public static List<GameObject> ballsack = new List<GameObject>(); // list of all balls in scene

    public static GameObject[] holes = new GameObject[4];

    public static float ballRadius;

    // gamestate stuff

    public static bool paused = false;
    public static float sfxvolume;
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

    public static bool hasScratched = false;

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
            if (ball == null)
            {
                continue;
            }

            totalVelocity += ball.GetComponent<Rigidbody>().velocity.sqrMagnitude;
        }
        return totalVelocity;
    }

    public static void haltBallMovement()
    {
        foreach (GameObject ball in ballsack)
        {
            if (ball == null)
            {
                continue;
            }

            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().rotation = Quaternion.identity;
        }
    }

    // ai behavior

    public static Vector3 directionToHit()
    {

        List<Vector3> candidates = new List<Vector3>();
        foreach (GameObject hole in holes)
        {
            foreach (GameObject ball in ballsack)
            {
                if (ball != ballsack[0])
                {
                    Ray ray = new Ray(ball.transform.position, hole.transform.position - ball.transform.position);
                    if (Physics.Raycast(ray, Vector3.Magnitude(hole.transform.position - ball.transform.position), 7))
                    {
                        continue;
                    }

                    Vector3 targetPoint = ray.GetPoint(-ballRadius * 2);

                    candidates.Add(targetPoint - ballsack[0].transform.position);
                }
            }
        }

        if (candidates.Count == 0)
        {
            float idkNum = Random.Range(0f, 2f * Mathf.PI);
            return new Vector3(Mathf.Sin(idkNum), 0, Mathf.Cos(idkNum));
        }



        //Vector3 shootVector = candidates[Random.Range(0, candidates.Count)];
        Vector3 shootVector = candidates[0];

        Debug.Log(shootVector + ballsack[0].transform.position);

        return shootVector;

        

        //return Mathf.Atan2(shootVector.x, -shootVector.z);

        // if no valid hit can be found, perform random hit
    }


    public static Vector3 scratchPlacement()
    {
        foreach (GameObject hole in holes)
        {
            foreach (GameObject ball in ballsack)
            {
                if (ball != ballsack[0]) 
                {
                    // check space between ball and hole
                    Ray ray = new Ray(ball.transform.position, hole.transform.position - ball.transform.position);
                    if (ballCast(ray, Vector3.Magnitude(hole.transform.position - ball.transform.position), 8))
                    {
                        continue;
                    }

                    // check space behind ball

                    Ray backRay = new Ray(ball.transform.position, ball.transform.position - hole.transform.position);
                    if (ballCast(ray, ballRadius * 4))
                    {
                        continue;
                    }

                    // if available get point behind ball

                    return backRay.GetPoint(ballRadius * 3);
                }
            }
        }

        // if no point works get a random free spot
        Debug.Log("tweaking out");
        return randomFreeSpot();
    }

    public static Vector3 randomFreeSpot()
    {
        Vector3 spot = new Vector3(Random.Range(-8.0f, 8.0f), 1, Random.Range(-8.0f, 8.0f));
        while (isSpotTaken(spot))
        {
            spot = new Vector3(Random.Range(-8.0f, 8.0f), 1, Random.Range(-8.0f, 8.0f));
        }

        return spot;
    }

    public static bool isSpotTaken(Vector3 spot)
    {
        foreach (GameObject ball in ballsack)
        {
            if (Vector3.Distance(ball.transform.position, spot) < ballRadius)
            {
                return false;
            }
        }
        return true;
    }

    public static GameObject getFastestBall()
    {
        int fastestIndex = 0;
        float fastest = 0;
        for (int i = 0; i < ballsack.Count; i++)
        {
            if (ballsack[i] == null)
            {
                continue;
            }

            if (Vector3.SqrMagnitude(ballsack[i].GetComponent<Rigidbody>().velocity) >= fastest)
            {
                fastest = Vector3.SqrMagnitude(ballsack[i].GetComponent<Rigidbody>().velocity);
                fastestIndex = i;
            }
        }

        return ballsack[fastestIndex];
    }


    // casting ball instead of just ray

    static bool ballCast(Ray ray, float maxDistance, int layerMask)
    {
        Ray leftRay = new Ray(ray.origin + ballRadius * Vector3.Cross(Vector3.Normalize(ray.direction), Vector3.up), ray.direction);
        Ray rightRay = new Ray(ray.origin + ballRadius * Vector3.Cross(Vector3.Normalize(ray.direction), Vector3.down), ray.direction);

        if (Physics.Raycast(ray, maxDistance, layerMask) || Physics.Raycast(leftRay, maxDistance - ballRadius, layerMask) || Physics.Raycast(rightRay, maxDistance - ballRadius, layerMask))
        {
            return true;
        }

        return false;
    }

    // overload for no layermask
    static bool ballCast(Ray ray, float maxDistance)
    {
        Ray leftRay = new Ray(ray.origin + ballRadius * Vector3.Cross(Vector3.Normalize(ray.direction), Vector3.up), ray.direction);
        Ray rightRay = new Ray(ray.origin + ballRadius * Vector3.Cross(Vector3.Normalize(ray.direction), Vector3.down), ray.direction);

        if (Physics.Raycast(ray, maxDistance) || Physics.Raycast(leftRay, maxDistance - ballRadius) || Physics.Raycast(rightRay, maxDistance - ballRadius))
        {
            return true;
        }

        return false;
    }
}
