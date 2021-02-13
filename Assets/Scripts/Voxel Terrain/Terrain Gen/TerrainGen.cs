using UnityEditor.SceneManagement;
using UnityEngine;
using SimplexNoise;

[System.Serializable]
public class TerrainGen
{
    public float stoneBaseHeight = -24;
    public float stoneBaseNoise = 0.05f;
    public float stoneBaseNoiseHeight = 4;
    public float stoneMountainHeight = 48;
    public float stoneMountainFrequency = 0.008f;
    public float stoneMinHeight = -12;
    public float dirtBaseHeight = 1;
    public float dirtNoise = 0.04f;
    public float dirtNoiseHeight = 3;
    public float caveFrequency = 0.025f;
    public int caveSize = 7;
    public int caveMax = 50;

    public Chunk ChunkGen(Chunk chunk)
    {
        for (var x = chunk.pos.x; x < chunk.pos.x + Chunk.chunkSize; x++)
        for (var z = chunk.pos.z; z < chunk.pos.z + Chunk.chunkSize; z++)
            chunk = ChunkColumnGen(chunk, x, z);

        return chunk;
    }

    public Chunk ChunkColumnGen(Chunk chunk, int x, int z)
    {
        int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);
        stoneHeight += GetNoise(x, 0, z, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight));
        if (stoneHeight < stoneMinHeight)
            stoneHeight = Mathf.FloorToInt(stoneMinHeight);
        stoneHeight += GetNoise(x, 0, z, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight));
        int dirtHeight = stoneHeight + Mathf.FloorToInt(dirtBaseHeight);
        dirtHeight += GetNoise(x, 100, z, dirtNoise, Mathf.FloorToInt(dirtNoiseHeight));

        for (int y = chunk.pos.y; y < chunk.pos.y + Chunk.chunkSize; y++)
        {
            int caveChance = GetNoise(x, y, z, caveFrequency, caveMax);
            if (y <= stoneHeight && caveSize < caveChance)
            {
                chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new Block(1));
            }
            else if (y <= dirtHeight && caveSize < caveChance)
            {
                chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new Block(1));
            }
            else
            {
                chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new Block(0));
            }
        }
        return chunk;
    }

    public static int GetNoise(int x, int y, int z, float scale, int max)
    {
        return Mathf.FloorToInt( (Noise.Generate(x * scale, y * scale, z * scale) + 1f) * (max/2f));
    }
}