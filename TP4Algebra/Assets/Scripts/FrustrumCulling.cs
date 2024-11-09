using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FrustrumCulling : MonoBehaviour
{
    [SerializeField] private float fov;
    [SerializeField] private float distance;
    [SerializeField] private float minDistance;

    [SerializeField] private Vector3 sizeP1;
    [SerializeField] private Vector3 sizeP2;
    [SerializeField] private Vector2 aspectRatio;

    [SerializeField] private Vector3 centerOfScreen;
    [SerializeField] private Vector3 centerOfScreen2;

    [SerializeField] private Vector3[] vertices;

    [SerializeField] private float tanAngle;

    [SerializeField] private List<GameObject> listObj = new List<GameObject>();
    [SerializeField] private List<Plane> planes = new List<Plane>();

    private void Awake()
    {
        vertices = new Vector3[8];

        Plane newP = new Plane(vertices[0], vertices[1], vertices[2], vertices[3]); // Front
        planes.Add(newP);

        Plane newP2 = new Plane(vertices[4], vertices[0], vertices[3], vertices[7]); // Left Plane
        planes.Add(newP2);

        Plane newP3 = new Plane(vertices[1], vertices[5], vertices[6], vertices[2]); // Right Plane
        planes.Add(newP3);

        Plane newP4 = new Plane(vertices[4], vertices[5], vertices[1], vertices[0]); // Top Plane
        planes.Add(newP4);

        Plane newP5 = new Plane(vertices[3], vertices[2], vertices[6], vertices[7]); // Down Plane
        planes.Add(newP5);

        Plane newP6 = new Plane(vertices[7], vertices[6], vertices[5], vertices[4]); // Back Plane
        planes.Add(newP6);
    }

    // Update is called once per frame
    void Update()
    {
        tanAngle = Mathf.Deg2Rad * fov;

        centerOfScreen = transform.position + (transform.forward * minDistance);
        centerOfScreen2 = transform.position + (transform.forward * distance);

        sizeP1.x = minDistance * Mathf.Tan(tanAngle / 2f);
        sizeP1.y = sizeP1.x * aspectRatio.y / aspectRatio.x;

        sizeP2.x = distance * Mathf.Tan(tanAngle / 2f);
        sizeP2.y = sizeP2.x * aspectRatio.y / aspectRatio.x;

        CalculateVertices();
        UpdateVerticesInPlanes();

        Culling();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        for (int i = 0; i < planes.Count; i++)
        {
            planes[i].DrawGizmo(transform);
        }
    }

    void CalculateVertices()
    {
        vertices[0] = transform.up * sizeP1.y + transform.right * -sizeP1.x + centerOfScreen;
        vertices[1] = transform.up * sizeP1.y + transform.right * sizeP1.x + centerOfScreen;
        vertices[2] = transform.up * -sizeP1.y + transform.right * sizeP1.x + centerOfScreen;
        vertices[3] = transform.up * -sizeP1.y + transform.right * -sizeP1.x + centerOfScreen;

        vertices[4] = transform.up * sizeP2.y + transform.right * -sizeP2.x + centerOfScreen2;
        vertices[5] = transform.up * sizeP2.y + transform.right * sizeP2.x + centerOfScreen2;
        vertices[6] = transform.up * -sizeP2.y + transform.right * sizeP2.x + centerOfScreen2;
        vertices[7] = transform.up * -sizeP2.y + transform.right * -sizeP2.x + centerOfScreen2;
    }
    void UpdateVerticesInPlanes()
    {
        planes[0].SetVertices(vertices[0], vertices[1], vertices[2], vertices[3]); // Front
        planes[1].SetVertices(vertices[4], vertices[0], vertices[3], vertices[7]); // Left
        planes[2].SetVertices(vertices[1], vertices[5], vertices[6], vertices[2]); // Right
        planes[3].SetVertices(vertices[4], vertices[5], vertices[1], vertices[0]); // Top
        planes[4].SetVertices(vertices[3], vertices[2], vertices[6], vertices[7]); // Down
        planes[5].SetVertices(vertices[7], vertices[6], vertices[5], vertices[4]); // Back
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
                        if (planes[k].IsPositiveToThePlane(aabb.BoxVertices[j]))
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
}
