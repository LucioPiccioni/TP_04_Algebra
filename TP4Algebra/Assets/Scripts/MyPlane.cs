using System;
using UnityEngine;

[Serializable]
public class MyPlane
{
    [SerializeField] private Vector3 normal;
    [SerializeField] private float distance;

    public Vector3 Normal { get { return normal; } }
    public float Distance { get { return distance; } }

    public MyPlane(Vector3 point, Vector3 normal)
    {
        this.normal = -normal.normalized;
        distance = -Vector3.Dot(this.normal, point);
    }

    public bool GetSide(Vector3 pointToCheck)
    {
        return Vector3.Dot(normal, pointToCheck) + distance > 0f;
    }

    public void SetNormalAndPosition(Vector3 point, Vector3 normal)
    {
        this.normal = -normal.normalized;
        distance = -Vector3.Dot(this.normal, point);
    }
}
