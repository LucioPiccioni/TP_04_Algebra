using System;
using UnityEngine;

[Serializable]
public class Plane
{
    [SerializeField] private Vector3 normal;
    [SerializeField] private Vector3 point;

    [SerializeField] private Vector3 center;

    [SerializeField] private Vector3[] vertices;

    public Vector3 Normal { get { return normal; } }
    public Vector3 Point { get { return point; } }
    public Vector3 Center { get { return center; } }

    public Plane(Vector3 normal, Vector3 point)
    {
        this.normal = normal;
        this.point = point;
    }

    public Plane(Vector3 vect1, Vector3 vect2, Vector3 vect3, Vector3 vect4)
    {
        point = vect1;
        
        vertices = new Vector3[4];
        vertices[0] = vect1;
        vertices[1] = vect2;
        vertices[2] = vect3;
        vertices[3] = vect4;

        CalculateNormal();
    }

    void CalculateNormal()
    {
        Vector3 vert1 = vertices[1] - vertices[0];
        Vector3 vert2 = vertices[2] - vertices[0];

        normal = GetCrossProduct(vert2, vert1).normalized;
    }

    public void SetVertices(Vector3 vect1, Vector3 vect2, Vector3 vect3, Vector3 vect4)
    {
        vertices[0] = vect1;
        vertices[1] = vect2;
        vertices[2] = vect3;
        vertices[3] = vect4;

        CalculateNormal();
    }

    public bool LiesOnPlane(Vector3 pointToCheck)
    {
        return normal.x * (pointToCheck.x - point.x) +
        normal.y * (pointToCheck.y - point.y) +
        normal.z * (pointToCheck.z - point.z) == 0;
    }

    public bool IsPositiveToThePlane(Vector3 pointToCheck)
    {
        bool isContain = true;

        float dot = (pointToCheck.x - point.x) * normal.x + (pointToCheck.y - point.y) * normal.y + (pointToCheck.z - point.z) * normal.z;

        if (dot < 0)
            isContain = false;

        return isContain;
    }

    public void DrawGizmo(Transform transform)
    {
        Vector3 v1 = vertices[0];
        Vector3 v2 = vertices[1];
        Vector3 v3 = vertices[2];
        Vector3 v4 = vertices[3];

        Gizmos.color = Color.green;
        Gizmos.DrawLine(v1, v2);
        Gizmos.DrawLine(v2, v3);
        Gizmos.DrawLine(v3, v4);
        Gizmos.DrawLine(v4, v1);

        Gizmos.color = Color.red;
        center = (v1 + v2 + v3 + v4) / vertices.Length;
        Gizmos.DrawLine(center, center + normal * 0.2f);
    }

    Vector3 GetCrossProduct(Vector3 rotationA, Vector3 rotationB)
    {
        Vector3 rotation;

        rotation.x = rotationA.y * rotationB.z - rotationA.z * rotationB.y;
        rotation.y = rotationA.z * rotationB.x - rotationA.x * rotationB.z;
        rotation.z = rotationA.x * rotationB.y - rotationA.y * rotationB.x;

        return rotation;
    }
}
