using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class StateHandler
{
    public static GameObject[] arrows = new GameObject[4];
    public static float pathSpacing = 0.1f;

    public enum GameState
    {
        SELECT_BALL,
        ROTATE_CUE,
        SHOOT_CUE,
        CUE_RELEASED,
        BALLS_MOVING
    }

    public static Ray[] paths = new Ray[2];
    public static float[] lengths = new float[2];

    public static GameState currentState = GameState.SELECT_BALL;

    public static GameObject predictionIndicator;

    private static List<GameObject> dingleList = new List<GameObject>();

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
            GameObject dingle = Object.Instantiate(predictionIndicator, paths[currentRayNum].GetPoint(currentLength), Quaternion.identity);
            dingle.GetComponent<Renderer>().material.color = new Color(1, 1, 0, (lengths[currentRayNum] - currentLength) / lengths.Sum());
            dingleList.Add(dingle);
            currentLength += pathSpacing;
            if (currentLength > lengths[currentRayNum])
            {
                currentLength = pathSpacing - (lengths[currentRayNum] - currentLength);
                currentRayNum++;
            }
        }
        
    }
}
