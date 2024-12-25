using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateThisOneBehaviorBecauseUnityIsAnnoying : MonoBehaviour
{
    public GameObject poolTable;
    public GameObject predictionIndicator;

    void Start()
    {
        StateHandler.predictionIndicator = predictionIndicator;
    }
    public void updateThisOneBehaviorBecauseUnityIsAnnoying()
    {
        StateHandler.shootCue();
    }
}
