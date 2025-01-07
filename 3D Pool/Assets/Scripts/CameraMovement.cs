using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    /*
    Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
    Converted to C# 27-02-13 - no credit wanted.
    Simple flycam I made, since I couldn't find any others made public.  
    Made simple to use (drag and drop, done) for regular keyboard layout  
    wasd : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/


    float mainSpeed = 10.0f; //regular speed
    float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 1000.0f; //Maximum speed when holdin gshift
    float camSens = 0.2f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    public float interpDamping = 5.0f;

    Pose[] camPoses = new Pose[3];
    static string[] camModes = new string[3];

    public static int currentPose = 2;


    // 2 variables for LITERALLY THE EXACT SAME THING, EXCEPT I HAD TO MAKE ONE STATIC.
    public GameObject camModeText;
    public static GameObject thisIsStupid;

    private void Start()
    {
        camPoses[0] = new Pose(new Vector3(0, 5, 0), Quaternion.Euler(0, 0, -90)); // track cam
        camPoses[1] = new Pose(new Vector3(0, 18, 0), Quaternion.Euler(90, 0, -90)); // static cam
        camPoses[2] = new Pose(gameObject.transform.position, gameObject.transform.rotation); // free cam

        camModes[0] = "Track";
        camModes[1] = "Static";
        camModes[2] = "Free";

        thisIsStupid = camModeText;
    }
    void Update()
    {
        // update cam position

        transform.position = Vector3.Slerp(transform.position, camPoses[currentPose].position, Time.deltaTime * interpDamping);
        transform.rotation = Quaternion.Slerp(transform.rotation, camPoses[currentPose].rotation, Time.deltaTime * interpDamping);

        // select camerapos

        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Tab))
        {
            currentPose--;
            if (currentPose < 0)
            {
                currentPose = camPoses.Length - 1;
            }

            updateCamText();
        }


        // other cam logic
        if (currentPose != 2)
        {
            if (StateHandler.ballsack[0] != null)
            {
                camPoses[0].rotation = Quaternion.LookRotation(StateHandler.ballsack[0].transform.position - transform.position);
            }
            else
            {
                camPoses[0].rotation = Quaternion.LookRotation(StateHandler.getFastestBall().transform.position - transform.position);
            }

            camPoses[2] = camPoses[0];

            return;
        }


        // free cam logic

        lastMouse = Input.mousePosition - lastMouse;
        if (Input.GetMouseButton(1))
        {
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(camPoses[2].rotation.eulerAngles.x + lastMouse.x, camPoses[2].rotation.eulerAngles.y + lastMouse.y, 0);
            camPoses[2].rotation = Quaternion.Euler(lastMouse);
            transform.eulerAngles = lastMouse;
        }
        

        lastMouse = Input.mousePosition;
        //Mouse  camera angle done.  

        //Keyboard commands
        Vector3 p = GetBaseInput();
        p = Quaternion.Euler(0, transform.eulerAngles.y, 0) * p;
        //Vector3 p_vertical = GetUpDownInput();
        if (p.sqrMagnitude > 0)
        { // only move while a direction key is pressed
            if (Input.GetKey(KeyCode.LeftControl))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p *= mainSpeed;
            }

            p *= Time.deltaTime;
            camPoses[2].position += p;
            // transform.Translate(p, Space.World);

        }
    }

    //private Vector3 GetUpDownInput()
    //{
    //    Vector3 p_Velocity = new Vector3();
    //    if (Input.GetKey(KeyCode.Space))
    //    {
    //        p_Velocity += new Vector3(0, 1, 0);
    //    }
    //    if (Input.GetKey(KeyCode.LeftShift))
    //    {
    //        p_Velocity += new Vector3(0, -1, 0);
    //    }
    //    return p_Velocity;
    //}

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        return p_Velocity;
    }

    public static void updateCamText()
    {
        thisIsStupid.GetComponent<TMP_Text>().text = "Camera Mode: " + camModes[currentPose];
    }
}
