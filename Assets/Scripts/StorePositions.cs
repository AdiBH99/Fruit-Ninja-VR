using System.Collections.Generic;
using UnityEngine;

public class StorePositions : MonoBehaviour
{
    public int maxPositions = 10; // Maximum number of positions to store
    private Queue<Vector3> positionsQueue = new Queue<Vector3>(); // Queue to store positions

    void Update()
    {
        // Store the current position of the sword
        Vector3 currentPosition = transform.position;

        // Add the current position to the queue
        positionsQueue.Enqueue(currentPosition);

        // If the queue exceeds the maximum number of positions, remove the oldest one
        if (positionsQueue.Count > maxPositions)
        {
            positionsQueue.Dequeue();
        }
    }

    // Function to get the stored positions
    public List<Vector3> GetStoredPositions()
    {
        return new List<Vector3>(positionsQueue);
    }

    // Function to create a mesh from the stored positions
    public Mesh CreateSlicingSurface()
    {
        List<Vector3> positions = GetStoredPositions();

        if (positions.Count < 2)
            return null;

        Mesh slicingSurface = new Mesh();

        // Create vertices
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < positions.Count - 1; i++)
        {
            Vector3 p1 = positions[i];
            Vector3 p2 = positions[i + 1];

            vertices.Add(p1);
            vertices.Add(p2);
        }

        // Create triangles
        for (int i = 0; i < vertices.Count - 2; i += 2)
        {
            int v1 = i;
            int v2 = i + 1;
            int v3 = i + 2;
            int v4 = i + 3;

            // First triangle
            triangles.Add(v1);
            triangles.Add(v2);
            triangles.Add(v3);

            // Second triangle
            triangles.Add(v2);
            triangles.Add(v4);
            triangles.Add(v3);
        }

        slicingSurface.vertices = vertices.ToArray();
        slicingSurface.triangles = triangles.ToArray();
        slicingSurface.RecalculateNormals();

        return slicingSurface;
    }
}
