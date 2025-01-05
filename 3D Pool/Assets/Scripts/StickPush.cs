using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickPush : MonoBehaviour
{
    Vector3 initMousePos;
    float maxDragDistance = 2f;
    float defaultDistance = -1.5f;
    bool released = false;
    float releaseAcceleration = 40f;
    public float releaseVelocity;
    float ballRadius;
    float thisSize;
    int ticksSinceChange = 0;

    // Update is called once per frame

    private void Start()
    {
        transform.localPosition = new Vector3(0, 0, defaultDistance);
        ballRadius = gameObject.GetComponentInParent<SphereCollider>().radius;
    }

    private Vector3 getObjectPos()
    {
        return Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0));
    }

    private void Update()
    {
        if (StateHandler.currentState != StateHandler.GameState.SHOOT_CUE && StateHandler.currentState != StateHandler.GameState.CUE_RELEASED)
        {
            ticksSinceChange = 0;
            return;
        }

        ticksSinceChange += 1;

        if (ticksSinceChange < 2)
        {
            return;
        }

        if (!StateHandler.paused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                initMousePos = Input.mousePosition - getObjectPos();
            }

            if (Input.GetMouseButton(0))
            {
                transform.localPosition = new Vector3(0, 0, scalingFunc(-Mathf.Abs(Camera.main.ScreenToWorldPoint(Input.mousePosition - initMousePos).z)));
            }

            if (Input.GetMouseButtonUp(0))
            {
                released = true;
                StateHandler.currentState = StateHandler.GameState.CUE_RELEASED;
            }
        }

        if (released)
        {
            transform.Translate(Vector3.up * releaseVelocity * Time.deltaTime);
            releaseVelocity += releaseAcceleration * Time.deltaTime;
        }

        if (transform.localPosition.z > -2 * ballRadius)
        {
            released = false;
            StateHandler.currentState = StateHandler.GameState.BALLS_MOVING;
            GetComponentInParent<Rigidbody>().velocity = transform.TransformVector(Vector3.up * releaseVelocity);
            GameObject.Find("boom").GetComponent<AudioSource>().Play();
            CameraMovement.currentPose = 0;
            CameraMovement.updateCamText();
            Destroy(gameObject);
        }
    }

    private float scalingFunc(float val)
    {
        if (val > 0)
        {
            //return 1 - Mathf.Exp(-val) + defaultDistance;
            return defaultDistance;
        } 
        else
        {
            return Mathf.Max(maxDragDistance * (Mathf.Exp(val) - 1), val) + defaultDistance;
        }
        
    }

    //private float getDragMovement()
    //{
    //    float adjustedMousePos = Input.mousePosition.x - Screen.width / 2;
    //    //float movement = Mathf.Exp(-Mathf.Pow(adjustedMousePos * camSens, 2));
    //    Vector3 movement = Input.mousePosition - getObjectPos();

    //    return movement;
    //}

    
    
}
