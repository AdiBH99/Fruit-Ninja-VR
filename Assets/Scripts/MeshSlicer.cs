using System.Collections.Generic;
using UnityEngine;

public static class MeshSlicer
{
    public static void SliceMesh(GameObject original, Plane slicePlane, out GameObject half1, out GameObject half2)
    {
        Mesh originalMesh = original.GetComponent<MeshFilter>().mesh;
        List<Vector3> vertices = new List<Vector3>(originalMesh.vertices);
        List<int> triangles = new List<int>(originalMesh.triangles);

        List<Vector3> half1Vertices = new List<Vector3>();
        List<int> half1Triangles = new List<int>();
        List<Vector3> half2Vertices = new List<Vector3>();
        List<int> half2Triangles = new List<int>();

        for (int i = 0; i < triangles.Count; i += 3)
        {
            int index1 = triangles[i];
            int index2 = triangles[i + 1];
            int index3 = triangles[i + 2];

            Vector3 v1 = vertices[index1];
            Vector3 v2 = vertices[index2];
            Vector3 v3 = vertices[index3];

            bool v1Side = slicePlane.GetSide(v1);
            bool v2Side = slicePlane.GetSide(v2);
            bool v3Side = slicePlane.GetSide(v3);

            if (v1Side == v2Side && v2Side == v3Side)
            {
                // All vertices on the same side
                if (v1Side)
                {
                    // All vertices on side 1
                    half1Vertices.Add(v1);
                    half1Vertices.Add(v2);
                    half1Vertices.Add(v3);

                    half1Triangles.Add(half1Vertices.Count - 3);
                    half1Triangles.Add(half1Vertices.Count - 2);
                    half1Triangles.Add(half1Vertices.Count - 1);
                }
                else
                {
                    // All vertices on side 2
                    half2Vertices.Add(v1);
                    half2Vertices.Add(v2);
                    half2Vertices.Add(v3);

                    half2Triangles.Add(half2Vertices.Count - 3);
                    half2Triangles.Add(half2Vertices.Count - 2);
                    half2Triangles.Add(half2Vertices.Count - 1);
                }
            }
            else
            {
                // Mixed side triangle (complex case)
                // You can add complex mesh slicing logic here
                // For simplicity, we'll ignore this case in this example
            }
        }

        // Create new meshes
        Mesh mesh1 = new Mesh();
        mesh1.vertices = half1Vertices.ToArray();
        mesh1.triangles = half1Triangles.ToArray();

        Mesh mesh2 = new Mesh();
        mesh2.vertices = half2Vertices.ToArray();
        mesh2.triangles = half2Triangles.ToArray();

        // Create new game object   
        half1 = new GameObject(original.name + "_Half1", typeof(MeshFilter), typeof(MeshRenderer));
        half2 = new GameObject(original.name + "_Half2", typeof(MeshFilter), typeof(MeshRenderer));

        half1.GetComponent<MeshFilter>().mesh = mesh1;
        half2.GetComponent<MeshFilter>().mesh = mesh2;

        // Copy original material
        half1.GetComponent<MeshRenderer>().material = original.GetComponent<MeshRenderer>().material;
        half2.GetComponent<MeshRenderer>().material = original.GetComponent<MeshRenderer>().material;

        // Optionally, add Rigidbody components if needed
        Rigidbody rb1 = half1.AddComponent<Rigidbody>();
        Rigidbody rb2 = half2.AddComponent<Rigidbody>();

        rb1.AddForce(slicePlane.normal * 5f, ForceMode.Impulse);
        rb2.AddForce(-slicePlane.normal * 5f, ForceMode.Impulse);
    }
}
