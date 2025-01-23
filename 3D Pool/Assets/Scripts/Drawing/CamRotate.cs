using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public Camera fakeCam;
    public float camDistance = 10f;

    public float camSpeed;

    Vector3 tableCenter;

    Vector3 lastMousePos;

    private float rotation;

    private float timeSinceClicked;

    // Start is called before the first frame update
    void Start()
    {
        rotation = camSpeed;
        tableCenter = fakeCam.ScreenToWorldPoint(new Vector3(Screen.width * 1 / 4, Screen.height / 2, 4));
    }

    // Update is called once per frame
    void Update()
    {
        // drag rotation around
        if (Input.GetMouseButton(0))
        {
            rotation = (lastMousePos.x - Input.mousePosition.x) * 30;
            timeSinceClicked = 0;
        }
        else
        // spin camera slowly
        {
            rotation = (1-Mathf.Clamp(timeSinceClicked / 10, 0, 1)) * (rotation - camSpeed) + camSpeed;
        }

        timeSinceClicked += Time.deltaTime;

        transform.Rotate(Vector3.down, Time.deltaTime * rotation, Space.World);

        // update camera position so that pool table is visible in center of the right side of screen

        Vector3 viewCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 3 / 4, Screen.height / 2, camDistance));

        transform.position += tableCenter - viewCenter;

        lastMousePos = Input.mousePosition;
    }
}
