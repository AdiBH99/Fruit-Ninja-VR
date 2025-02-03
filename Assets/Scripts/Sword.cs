using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using EzySlice;

public class Sword : MonoBehaviour
{
    public Queue<Vector3> swordPositions = new Queue<Vector3>();

    public Vector3 last_position;
    public const int maxPositions = 3;
    public UnityEngine.Plane swordPlane;

    void Update()
    {
        // Update the sword's position queue
        UpdateSwordPositions();
        // Calculate the sword plane
        if (swordPositions.Count == maxPositions)
        {
            swordPlane = CalculatePlane(swordPositions.ToArray());
        }
    }

    public UnityEngine.Plane GetSlicingPlane()
    {
        return swordPlane;
    }

    void UpdateSwordPositions()
    {
        // Add current position to the queue
        Vector3 position = this.gameObject.transform.position;
        if (position == last_position) {
            Vector3 offset = new Vector3(0.01f, -0.1f, 0.01f);
            position = last_position + offset;
        }
        swordPositions.Enqueue(position);
        // Maintain only the last 5 positions
        if (swordPositions.Count > maxPositions)
        {
            swordPositions.Dequeue();
        }
        last_position = position;
    }

    UnityEngine.Plane CalculatePlane(Vector3[] positions) {
        // Dequeue three vectors
        // Vector3 a = swordPositions.Dequeue();
        // Vector3 b = swordPositions.Dequeue();
        // Vector3 c = swordPositions.Dequeue();

        // // Calculate the plane
        // UnityEngine.Plane plane = new UnityEngine.Plane(a, b, c);

        // // Enqueue the vectors back to the queue
        // swordPositions.Enqueue(a);
        // swordPositions.Enqueue(b);
        // swordPositions.Enqueue(c);
        // return plane;
        UnityEngine.Plane cuttingPlane = new UnityEngine.Plane();

		// the plane will be set to the same coordinates as the object that this
		// script is attached to
		// NOTE -> Debug Gizmo drawing only works if we pass the transform
		// cuttingPlane.Compute(this.gameObject.transform);

		// draw gizmos for the plane
		// NOTE -> Debug Gizmo drawing is ONLY available in editor mode. Do NOT try
		// to run this in the final build or you'll get crashes (most likey)
		// cuttingPlane.OnDebugDraw();
        return cuttingPlane;
    }


    UnityEngine.Plane CalculatePlaneOld(Vector3[] positions)
    {
        Vector3 centroid = Vector3.zero;
        foreach (var pos in positions)
        {
            centroid += pos;
        }
        centroid /= positions.Length;

        float[,] matrix = new float[3, 3];
        foreach (var pos in positions)
        {
            Vector3 diff = pos - centroid;
            matrix[0, 0] += diff.x * diff.x;
            matrix[0, 1] += diff.x * diff.y;
            matrix[0, 2] += diff.x * diff.z;
            matrix[1, 1] += diff.y * diff.y;
            matrix[1, 2] += diff.y * diff.z;
            matrix[2, 2] += diff.z * diff.z;
        }

        matrix[1, 0] = matrix[0, 1];
        matrix[2, 0] = matrix[0, 2];
        matrix[2, 1] = matrix[1, 2];

        float[,] eigenVectors = new float[3, 3];
        float[] eigenValues = new float[3];
        EigenDecomposition(matrix, eigenVectors, eigenValues);

        int minIndex = 0;
        if (eigenValues[1] < eigenValues[minIndex]) minIndex = 1;
        if (eigenValues[2] < eigenValues[minIndex]) minIndex = 2;

        Vector3 normal = new Vector3(eigenVectors[0, minIndex], eigenVectors[1, minIndex], eigenVectors[2, minIndex]);
        return new UnityEngine.Plane(normal, centroid);
    }

    void EigenDecomposition(float[,] matrix, float[,] eigenVectors, float[] eigenValues, int maxIterations = 1000)
    {
        int n = matrix.GetLength(0);
        float[,] a = (float[,])matrix.Clone();
        eigenVectors = IdentityMatrix(n);

        for (int k = 0; k < maxIterations; k++)
        {
            float[,] q = new float[n, n];
            float[,] r = new float[n, n];
            QRDecomposition(a, q, r);
            a = MultiplyMatrices(r, q);
            eigenVectors = MultiplyMatrices(eigenVectors, q);
        }

        for (int i = 0; i < n; i++)
        {
            eigenValues[i] = a[i, i];
        }
    }

    float[,] IdentityMatrix(int n)
    {
        float[,] identity = new float[n, n];
        for (int i = 0; i < n; i++)
        {
            identity[i, i] = 1.0f;
        }
        return identity;
    }

    void QRDecomposition(float[,] a, float[,] q, float[,] r)
    {
        int n = a.GetLength(0);
        float[,] ai = (float[,])a.Clone();
        float[,] qi = IdentityMatrix(n);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                r[j, i] = DotProduct(GetColumn(ai, j), GetColumn(ai, i));
            }
            float[] v = GetColumn(ai, i);
            for (int j = 0; j < i; j++)
            {
                v = SubtractVectors(v, ScaleVector(GetColumn(ai, j), r[j, i]));
            }
            r[i, i] = VectorNorm(v);
            float[] qiColumn = ScaleVector(v, 1.0f / r[i, i]);
            for (int j = 0; j < n; j++)
            {
                q[j, i] = qiColumn[j];
            }
            ai = SubtractMatrices(ai, OuterProduct(qiColumn, ScaleVector(qiColumn, r[i, i])));
        }
    }

    float[] GetColumn(float[,] matrix, int col)
    {
        int n = matrix.GetLength(0);
        float[] column = new float[n];
        for (int i = 0; i < n; i++)
        {
            column[i] = matrix[i, col];
        }
        return column;
    }

    float DotProduct(float[] v1, float[] v2)
    {
        float dot = 0;
        for (int i = 0; i < v1.Length; i++)
        {
            dot += v1[i] * v2[i];
        }
        return dot;
    }

    float[] ScaleVector(float[] vector, float scalar)
    {
        float[] result = new float[vector.Length];
        for (int i = 0; i < vector.Length; i++)
        {
            result[i] = vector[i] * scalar;
        }
        return result;
    }

    float[] SubtractVectors(float[] v1, float[] v2)
    {
        float[] result = new float[v1.Length];
        for (int i = 0; i < v1.Length; i++)
        {
            result[i] = v1[i] - v2[i];
        }
        return result;
    }

    float VectorNorm(float[] vector)
    {
        return (float)Mathf.Sqrt(DotProduct(vector, vector));
    }

    float[,] OuterProduct(float[] v1, float[] v2)
    {
        int n = v1.Length;
        float[,] result = new float[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[i, j] = v1[i] * v2[j];
            }
        }
        return result;
    }

    float[,] SubtractMatrices(float[,] m1, float[,] m2)
    {
        int n = m1.GetLength(0);
        float[,] result = new float[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[i, j] = m1[i, j] - m2[i, j];
            }
        }
        return result;
    }

    float[,] MultiplyMatrices(float[,] m1, float[,] m2)
    {
        int n = m1.GetLength(0);
        float[,] result = new float[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[i, j] = 0;
                for (int k = 0; k < n; k++)
                {
                    result[i, j] += m1[i, k] * m2[k, j];
                }
            }
        }
        return result;
    }
}
