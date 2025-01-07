using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public Vector3 start;
    public Vector3 end;
    float lineWidth = 0.15f;
    
    public static GameObject linePrefab;
    private GameObject displayLine;

    public Line(Vector3 point, Transform parent) : this(point, point, parent) { }
    
    public Line(Vector3 start, Vector3 end, Transform parent)
    {
        this.start = start;
        this.end = end;
        displayLine = Object.Instantiate(linePrefab, (start + end) / 2, Quaternion.identity, parent);
        displayLine.transform.localScale = new Vector3(lineWidth, Vector3.Magnitude(start - end) / 100 + lineWidth);
    }

    public void UpdateLine(Vector3 pos, bool isStart)
    {
        if (isStart)
        {
            start = pos;
        }
        else
        {
            end = pos;
        }

        displayLine.transform.position = (start + end) / 2;
        displayLine.transform.localScale = new Vector3(lineWidth, Vector3.Magnitude(start - end) / 100 + lineWidth);
        Vector3 upVector = end - start;
        if (end == start)
        {
            upVector = Vector3.up;
        }
        displayLine.transform.rotation = Quaternion.LookRotation(Vector3.back, upVector);
    }
}
