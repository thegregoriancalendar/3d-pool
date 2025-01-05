using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pose
{
    public Vector3 position;
    public Quaternion rotation;
    public Pose(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
