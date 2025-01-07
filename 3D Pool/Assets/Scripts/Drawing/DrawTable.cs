using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class DrawTable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject linePrefab;

    List<Line> lines = new List<Line>();
    Line currentLine;

    public EventSystem eventSys;

    public bool addLineMode = false; // figure out getters and setters at some point pls

    private bool mouseIsOver;

    float distanceTolerance = 10f;

    public GameObject highlightCirclePrefab;
    GameObject highlightCircle;

    // Start is called before the first frame update
    void Start()
    {
        Line.linePrefab = linePrefab;
    }

    // Update is called once per frame

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIsOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseIsOver = false;
    }

    void Update()
    {
        
        if (addLineMode && mouseIsOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                lines.Add(new Line(Input.mousePosition, transform));
            }

            if (Input.GetMouseButton(0))
            {
                lines[lines.Count - 1].UpdateLine(Input.mousePosition, false);
            }

            if (Input.GetMouseButtonUp(0))
            {
                addLineMode = false;
            }
        }

        // point to highlight

        (Line, bool) hiPoint = cursorNearEndPoint();

        Debug.Log(hiPoint);

        if (hiPoint.Item1 != null && highlightCircle == null)
        {
            if (hiPoint.Item2)
            {
                highlightCircle = Instantiate(highlightCirclePrefab, hiPoint.Item1.start, Quaternion.identity);
            }
            else
            {
                highlightCircle = Instantiate(highlightCirclePrefab, hiPoint.Item1.end, Quaternion.identity);
            }
        } 
        else
        {
            if (highlightCircle != null)
            {
                Destroy(highlightCircle);
                highlightCircle = null;
            }
        }
    }

    public void setLineMode()
    {
        addLineMode = true;
    }

    (Line, bool) cursorNearEndPoint()
    {
        if (!mouseIsOver)
        {
            return (null, false);
        }

        foreach (Line line in lines)
        {
            if (Vector3.Distance(Input.mousePosition, line.start) < distanceTolerance)
            {
                return (line, true);
            }

            if (Vector3.Distance(Input.mousePosition, line.end) < distanceTolerance)
            {
                return (line, false);
            }
        }

        return (null, false);
    }
}
