using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ArrowDrag : MonoBehaviour
{
    static float defaultSize = 0.2f;
    static float highlightSize = 0.3f;
    Vector3 initMousePos;
    static float arrowDistance = 2.5f;
    public GameObject otherArrow;
    public static GameObject queueBall;
    static bool dragged = false;
    public bool isRightArrow;
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(0, 0, -arrowDistance);
        StateHandler.addArrow(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mouseOverObject())
            {
                Debug.Log("mouse down");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (mouseOverObject())
            {
                Debug.Log("mouse up");
            }
        }

        if (!dragged)
        {
            transform.position = queueBall.transform.position + offset;
        }
    }

    private bool mouseOverObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, Mathf.Infinity, 0);
    }

    private Vector3 getObjectPos()
    {
        return Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0));
    }   

    private void OnMouseDown()
    {
        transform.localScale = new Vector3(highlightSize, highlightSize, highlightSize);
        initMousePos = Input.mousePosition - getObjectPos();
        dragged = true;
        //thing = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        //thing2 = Instantiate(prefab, Vector3.zero, Quaternion.identity);
    }

    private void OnMouseDrag()
    {
        // finds ray from camera to point where mouse is and then intersects that with the plane on which the drag arrow lies to then find point to try to go to
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(new Vector3(0, 1, 0), queueBall.transform.position);
        float enter;
        Vector3 intersectPoint = Vector3.back;
        if (dragPlane.Raycast(mouseRay, out enter))
        {
            intersectPoint = mouseRay.GetPoint(enter);
        }

        //thing2.transform.position = intersectPoint;

        // finds closest point on "unit" circle and then places the arrow at that point

        Ray positionRay = new Ray(queueBall.transform.position, intersectPoint - queueBall.transform.position);
        transform.LookAt(queueBall.transform.position);
        otherArrow.transform.LookAt(queueBall.transform.position);
        int rotationCorrection = 180;
        if (isRightArrow)
        {
            rotationCorrection = 0;
        }
        transform.Rotate(new Vector3(-90 + rotationCorrection, rotationCorrection, 180));
        otherArrow.transform.Rotate(new Vector3(-90 + 180 - rotationCorrection, 180 - rotationCorrection, 180));

        transform.position = positionRay.GetPoint(arrowDistance);
        otherArrow.transform.position = positionRay.GetPoint(arrowDistance);

        queueBall.transform.LookAt(positionRay.GetPoint(-1));
        //thing.transform.position = positionRay.GetPoint(arrowDistance);
        //Debug.Log(transform.parent);

        updatePathPlanner();
    }

    private void OnMouseUp()
    {
        transform.localScale = new Vector3(defaultSize, defaultSize, defaultSize);
        dragged = false;
        offset = transform.position - queueBall.transform.position;
        otherArrow.GetComponent<ArrowDrag>().offset = transform.position - queueBall.transform.position;
    }

    private void updatePathPlanner()
    {
        StateHandler.paths[0].origin = queueBall.transform.position;
        StateHandler.paths[0].direction = queueBall.transform.position - transform.position;
        StateHandler.updatePaths();
        StateHandler.drawPaths();
    }
}
