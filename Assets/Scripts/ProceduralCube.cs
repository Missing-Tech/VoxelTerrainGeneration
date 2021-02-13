using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCube : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;

    public float scale = 1f;

    public Vector3 pos;
    
    private float adjustedScale;
    
    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        adjustedScale = scale * 0.5f;
    }

    private void Start()
    {
        
        MakeCube(adjustedScale, pos * scale);
        UpdateMesh();
    }

    void MakeCube(float cubeScale, Vector3 cubePos)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            MakeFace(i, cubeScale, cubePos);
        }
    }

    void MakeFace(int dir, float faceScale, Vector3 facePos)
    {
        vertices.AddRange(CubeMeshData.faceVertices(dir,faceScale, facePos));
        int vCount = vertices.Count;
        
        triangles.Add(vCount - 4);
        triangles.Add(vCount - 3);
        triangles.Add(vCount - 2);
        triangles.Add(vCount - 4);
        triangles.Add(vCount - 2);
        triangles.Add(vCount - 1);
    }
    
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
