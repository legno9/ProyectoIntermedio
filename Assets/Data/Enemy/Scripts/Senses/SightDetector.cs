using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SightDetector : MonoBehaviour
{
    [Header ("SightParameters")]
    [SerializeField] private float distance = 10f;
    [SerializeField] private float _angle = 90f;
    private float halfAngle
    {
        get => _angle/2 ; 
        set => _angle = value;
    }
    [SerializeField] private float sightHeight = 1f;
    [SerializeField] private float height = 1f;
    [SerializeField] private Color color = Color.red;

    [Header ("Detection")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask occlusionLayers;

    [Header ("Objects")]
    public List<GameObject> objects = new();

    private Mesh mesh;
    Collider[] colliders = new Collider[50];
    int count;

    [Header("Sounds")]
    [SerializeField] private AudioClipList detectSounds;
    public bool targetDetected = false;

    public bool Detect(GameObject target)
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layerMask, QueryTriggerInteraction.Collide);

        objects.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if (obj == gameObject){continue;}
                    
            if (IsInSight(obj))
            {
                objects.Add(obj);
                if (obj == target)
                {
                    if (!targetDetected)
                    {
                        detectSounds.PlayAtPointRandom(transform.position);
                        targetDetected = true;
                        if (TryGetComponent<HeardDetector>(out var heard))
                        {
                            heard.targetDetected = true;
                        }
                    }
                    return true;
                }
                else if (targetDetected)
                {
                    targetDetected = false;
                    if (TryGetComponent<HeardDetector>(out var heard))
                    {
                        heard.targetDetected = false;
                    }
                }
            }
        }

        return false;
    }

    private bool IsInSight(GameObject obj)
    {
        float halfHeight = height / 2;
        Vector3 origin = sightHeight * Vector3.up + transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;

        if (direction.y < -halfHeight || direction.y > halfHeight)
        {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(transform.forward, direction);
        if (deltaAngle > halfAngle)
        {
            return false;
        }

        if (Physics.Linecast(origin, dest, out RaycastHit hit, occlusionLayers))
        {
            if (hit.collider.gameObject != obj)
            {
                return false;
            }
            return true;
        }

        return true;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new ();
        
        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 forward = Vector3.forward * distance;
        Vector3 heightOffset = Vector3.up * (sightHeight - height/2);

        Vector3 bottomCenter = Vector3.zero + Vector3.up * sightHeight;
        Vector3 bottomLeft = Quaternion.Euler(0, -halfAngle, 0) * (forward + heightOffset);
        Vector3 bottomRight = Quaternion.Euler(0, halfAngle, 0) * (forward + heightOffset);

        Vector3 topCenter = bottomCenter;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vert = 0;

        // Left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // Right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -halfAngle;
        float deltaAngle = halfAngle * 2 / segments;

        for (int i = 0; i < segments; i++)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * (forward + heightOffset);
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * (forward + heightOffset);

            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;

            // Far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // Top side
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // Bottom side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate() 
    {
        mesh = CreateWedgeMesh();
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = color;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }
    }
}
