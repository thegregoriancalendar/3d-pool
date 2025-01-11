using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public Camera fakeCam;
    public float camDistance = 10f;

    public float camSpeed;

    Vector3 tableCenter;

    // Start is called before the first frame update
    void Start()
    {
        tableCenter = fakeCam.ScreenToWorldPoint(new Vector3(Screen.width * 1 / 4, Screen.height / 2, 4));
    }

    // Update is called once per frame
    void Update()
    {
        // spin camera slowly
        transform.Rotate(Vector3.down, Time.deltaTime * camSpeed, Space.World);

        // update camera position so that pool table is visible in center of the right side of screen

        Vector3 viewCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 3 / 4, Screen.height / 2, camDistance));

        transform.position += tableCenter - viewCenter;
    }
}
