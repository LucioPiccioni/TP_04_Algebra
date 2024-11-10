using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FrustrumCulling : MonoBehaviour
{
    [SerializeField] private float fov;
    [SerializeField] private float farDistance;
    [SerializeField] private float nearDistance;
    [SerializeField] private float size;
    [SerializeField] private float tanAngle;

    [SerializeField] private Vector3 sizeP1;
    [SerializeField] private Vector3 sizeP2;
    [SerializeField] private Vector2 aspectRatio;

    [SerializeField] private List<GameObject> listObj = new List<GameObject>();
    [SerializeField] private List<MyPlane> planes = new List<MyPlane>();

    private void OnEnable()
    {
        Vector3 frontNear = nearDistance * transform.forward;
        Vector3 frontFar = farDistance * transform.forward;

        tanAngle = Mathf.Deg2Rad * fov;
        float vertical = farDistance * Mathf.Tan(tanAngle * 0.5f);
        float horizontal = vertical * aspectRatio.y / aspectRatio.x;

        MyPlane near = new MyPlane(transform.position + frontFar, transform.forward);
        MyPlane far = new MyPlane(transform.position + frontNear, -transform.forward);
        MyPlane right = new MyPlane(transform.position, Vector3.Cross(transform.up, frontNear + transform.right * horizontal));
        MyPlane left = new MyPlane(transform.position, Vector3.Cross(frontNear - transform.right * horizontal, transform.up));
        MyPlane top = new MyPlane(transform.position, Vector3.Cross(transform.right, frontNear - transform.up * vertical));
        MyPlane bottom = new MyPlane(transform.position, Vector3.Cross(frontNear + transform.up * vertical, transform.right));

        planes.Add(near);
        planes.Add(far);
        planes.Add(right);
        planes.Add(left);
        planes.Add(top);
        planes.Add(bottom);
    }

    void Update()
    {
        tanAngle = Mathf.Deg2Rad * fov;

        UpdatePlanes();

        Culling();
    }

    private void OnDrawGizmos()
    {
        DrawFrustrum(planes);
    }

    void UpdatePlanes()
    {
        Vector3 frontFar = nearDistance * transform.forward;
        Vector3 frontNear = farDistance * transform.forward;

        tanAngle = Mathf.Deg2Rad * fov;
        float horizontal = nearDistance * Mathf.Tan(tanAngle * 0.5f);
        float vertical = horizontal * aspectRatio.y / aspectRatio.x;

        planes[0].SetNormalAndPosition(transform.position + frontNear, transform.forward);
        planes[1].SetNormalAndPosition(transform.position + frontFar, -transform.forward);
        planes[2].SetNormalAndPosition(transform.position, Vector3.Cross(transform.up, frontFar + transform.right * horizontal));
        planes[3].SetNormalAndPosition(transform.position, Vector3.Cross(frontFar - transform.right * horizontal, transform.up));
        planes[4].SetNormalAndPosition(transform.position, Vector3.Cross(transform.right, frontFar - transform.up * vertical));
        planes[5].SetNormalAndPosition(transform.position, Vector3.Cross(frontFar + transform.up * vertical, transform.right));
    }

    void Culling()
    {
        for (int i = 0; i < listObj.Count; i++)
        {
            AABB aabb = listObj[i].GetComponent<AABB>();
            MeshRenderer mR = listObj[i].GetComponentInChildren<MeshRenderer>();

            if (aabb && mR)
            {
                bool show = false;

                for (int j = 0; j < aabb.BoxVertices.Length; j++)
                {
                    int isPosAllPlanes = planes.Count;

                    for (int k = 0; k < planes.Count; k++)
                    {
                        if (planes[k].GetSide(aabb.BoxVertices[j]))
                            isPosAllPlanes--;
                    }

                    if (isPosAllPlanes <= 0)
                    {
                        show = true;
                        break;
                    }
                }

                mR.enabled = show;
            }
        }
    }

    private void DrawFrustrum(List<MyPlane> frustumPlanes) // Chat GPT ME AYUDO CON EL DIBUJADO
    {
        if (frustumPlanes.Count != 6) return;

        Vector3[] corners = new Vector3[8];
        corners[0] = IntersectThreePlanes(frustumPlanes[0], frustumPlanes[2], frustumPlanes[4]); // Near Top Left
        corners[1] = IntersectThreePlanes(frustumPlanes[1], frustumPlanes[2], frustumPlanes[4]); // Near Top Right
        corners[2] = IntersectThreePlanes(frustumPlanes[1], frustumPlanes[3], frustumPlanes[4]); // Near Bottom Right
        corners[3] = IntersectThreePlanes(frustumPlanes[0], frustumPlanes[3], frustumPlanes[4]); // Near Bottom Left
        corners[4] = IntersectThreePlanes(frustumPlanes[0], frustumPlanes[2], frustumPlanes[5]); // Far Top Left
        corners[5] = IntersectThreePlanes(frustumPlanes[1], frustumPlanes[2], frustumPlanes[5]); // Far Top Right
        corners[6] = IntersectThreePlanes(frustumPlanes[1], frustumPlanes[3], frustumPlanes[5]); // Far Bottom Right
        corners[7] = IntersectThreePlanes(frustumPlanes[0], frustumPlanes[3], frustumPlanes[5]); // Far Bottom Left

        Gizmos.color = Color.green;
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            Gizmos.DrawLine(corners[i + 4], corners[(i + 1) % 4 + 4]);
            Gizmos.DrawLine(corners[i], corners[i + 4]);
        }
    }

    private Vector3 IntersectThreePlanes(MyPlane p1, MyPlane p2, MyPlane p3)
    {
        Vector3 n1 = p1.Normal, n2 = p2.Normal, n3 = p3.Normal;
        float determinant = Vector3.Dot(n1, Vector3.Cross(n2, n3));

        if (Mathf.Abs(determinant) < 1e-6f)
        {
            Debug.LogWarning("Planes do not intersect at a single point.");
            return Vector3.zero;
        }

        Vector3 intersectPoint = (
            (-p1.Distance * Vector3.Cross(n2, n3)) +
            (-p2.Distance * Vector3.Cross(n3, n1)) +
            (-p3.Distance * Vector3.Cross(n1, n2))
        ) / determinant;

        return intersectPoint;
    }
}


