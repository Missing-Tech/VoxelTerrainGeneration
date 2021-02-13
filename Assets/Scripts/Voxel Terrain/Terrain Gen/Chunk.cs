using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public static int chunkSize = 16;

    private float _adjustedScale;
    private MeshCollider _col;
    private Mesh _mesh;
    private List<int> _triangles;
    private List<Vector3> _vertices;

    private List<Vector3> _voxelPositions;
    [FormerlySerializedAs("_world")] public World world;
    public Block[,,] blocks = new Block[chunkSize, chunkSize, chunkSize];
    public WorldPos pos;
    public bool rendered;

    public float scale = 1f;

    public bool update;

    private void Awake()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _col = GetComponent<MeshCollider>();
        _adjustedScale = scale * 0.5f;
    }

    private void Start()
    {
        GenerateVoxelMesh(new VoxelData());
    }

    private void LateUpdate()
    {
        if (update)
        {
            GenerateVoxelMesh(new VoxelData());
            update = false;
        }
    }

    public void SetBlocksUnmodified()
    {
        foreach (Block block in blocks)
        {
            block.changed = false;
        }
    }
    
    public Block GetBlock(int x, int y, int z)
    {
        if (InRange(x) && InRange(y) && InRange(z))
            return blocks[x, y, z];
        return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        if (InRange(x) && InRange(y) && InRange(z))
            blocks[x, y, z] = block;
        else
            world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
    }

    //new function
    public static bool InRange(int index)
    {
        if (index < 0 || index >= chunkSize)
            return false;

        return true;
    }

    public void GenerateVoxelMesh(VoxelData data)
    {
        rendered = true;
        _vertices = new List<Vector3>();
        _triangles = new List<int>();

        for (var x = 0; x < chunkSize; x++)
        for (var y = 0; y < chunkSize; y++)
        for (var z = 0; z < chunkSize; z++)
        {
            //var chunkPos = new Vector3Int(x + chunkX, y + chunkY, z + chunkZ);
            if (data.GetCell(x, y, z, this) == 0)
                continue;
            MakeCube(_adjustedScale, new Vector3(x, y, z), x, y, z, data);
        }

        UpdateMesh();
    }

    private void MakeCube(float cubeScale, Vector3 cubePos, int x, int y, int z, VoxelData data)
    {
        for (var i = 0; i < 6; i++)
            if (data.GetNeighbour(x, y, z, pos.x, pos.y, pos.z, (Direction) i, this) == 0)
                MakeFace((Direction) i, cubeScale, cubePos);
    }

    private void MakeFace(Direction dir, float faceScale, Vector3 facePos)
    {
        _vertices.AddRange(CubeMeshData.faceVertices(dir, faceScale, facePos));
        var vCount = _vertices.Count;

        _triangles.Add(vCount - 4);
        _triangles.Add(vCount - 3);
        _triangles.Add(vCount - 2);
        _triangles.Add(vCount - 4);
        _triangles.Add(vCount - 2);
        _triangles.Add(vCount - 1);
    }

    private void UpdateMesh()
    {
        _mesh.Clear();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.RecalculateBounds();
        _mesh.Optimize();
        _mesh.RecalculateNormals();
        _col.sharedMesh = null;
        _col.sharedMesh = _mesh;
    }
}