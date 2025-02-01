using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BallSelect : MonoBehaviour
{
    public GameObject stickPrefab;
    public GameObject leftArrowPrefab;
    public GameObject rightArrowPrefab;

    public GameObject stick;
    public GameObject leftArrow;
    public GameObject rightArrow;

    public float botMovementDamping = 1f;
    public float botTolerance = 0.04f; // angular tolerance for bot to stop rotating (in something that isn't degrees)

    private void OnMouseDown()
    {
        selectTheBall();
    }

    public void selectTheBall()
    {
        if (StateHandler.currentState == StateHandler.GameState.SELECT_BALL)
        {
            stick = Instantiate(stickPrefab, transform);
            rightArrow = Instantiate(rightArrowPrefab, transform.position + new Vector3(0, 0, -2.5f), Quaternion.Euler(new Vector3(-90, 0, 180)));
            leftArrow = Instantiate(leftArrowPrefab, transform.position + new Vector3(0, 0, -2.5f), Quaternion.Euler(new Vector3(90, 180, 180)));
            ArrowDrag.queueBall = gameObject;
            rightArrow.GetComponent<ArrowDrag>().otherArrow = leftArrow;
            leftArrow.GetComponent<ArrowDrag>().otherArrow = rightArrow;
            StateHandler.currentState = StateHandler.GameState.ROTATE_CUE;
        }
    }

    public bool botMove(Vector3 directionVector)
    {
        // direction is in radians
        //Vector3 directionVector = new Vector3(Mathf.Sin(direction), 0, Mathf.Cos(direction));
        //Debug.Log(direction.ToString() + " " + directionVector.ToString());

        Quaternion leftOffset = Quaternion.Euler(90, 180, 180);
        Quaternion rightOffset = Quaternion.Euler(-90, 0, 180);

        // gets appropriate rotation using slerp then bases position and everything off of that
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionVector), Time.deltaTime * botMovementDamping);
        //transform.rotation = Quaternion.LookRotation(directionVector);

        Vector3 lookDirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y));
        Ray positionRay = new Ray(transform.position, lookDirection);

        rightArrow.transform.rotation = transform.rotation * rightOffset;
        leftArrow.transform.rotation = transform.rotation * leftOffset;

        rightArrow.transform.position = positionRay.GetPoint(-ArrowDrag.arrowDistance);
        leftArrow.transform.position = positionRay.GetPoint(-ArrowDrag.arrowDistance);

        rightArrow.GetComponent<ArrowDrag>().updatePathPlanner();

        //Debug.Log(Vector3.Dot(directionVector, lookDirection));
        //Debug.Log(directionVector.magnitude * lookDirection.magnitude);

        if (Mathf.Abs(Vector3.Dot(directionVector, lookDirection) - directionVector.magnitude * lookDirection.magnitude) < botTolerance)
        {
            return true;
        }

        //Debug.Log(transform.rotation.eulerAngles.y);
        return false;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (StateHandler.xiJinping.Contains(collision.gameObject))
        {
            GameObject miniPing = Instantiate(collision.gameObject, collision.transform.position += new Vector3(1, 0, 1), Quaternion.identity);
            Destroy(miniPing.GetComponent<BallSelect>());
            StateHandler.xiJinping.Add(miniPing);

            GameObject miniPing2 = Instantiate(collision.gameObject, collision.transform.position += new Vector3(-1, 0, -1), Quaternion.identity);
            Destroy(miniPing2.GetComponent<BallSelect>());
            StateHandler.xiJinping.Add(miniPing2);

        }
    }
}
