using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class World : MonoBehaviour
{
    public GameObject chunk;
    [FormerlySerializedAs("chunksDictionary")] public Dictionary<WorldPos, Chunk> chunks = new Dictionary<WorldPos, Chunk>();
    public int chunkSize = 16;
    public string worldName = "world";
    public Transform player;
    public TerrainGen tg;
    
    public void CreateChunk(int x, int y, int z)
    {
        WorldPos worldPos = new WorldPos(x, y, z);

        //Instantiate the chunk at the coordinates using the chunk prefab
        GameObject newChunkObject = Instantiate(
            chunk, new Vector3(x, y, z),
            Quaternion.Euler(Vector3.zero)
        ) as GameObject;

        Chunk newChunk = newChunkObject.GetComponent<Chunk>();

        newChunk.pos = worldPos;
        newChunk.world = this;

        //Add it to the chunks dictionary with the position as the key
        chunks.Add(worldPos, newChunk);

        var terrainGen = new TerrainGen();
        newChunk = terrainGen.ChunkGen(newChunk);
        newChunk.SetBlocksUnmodified();
        Serialization.Load(newChunk);
    }

    private void OnApplicationQuit()
    {
        foreach (var chunk in chunks.Values)
        {
            Serialization.SaveChunk(chunk);
        }
    }

    public Chunk GetChunk(int x, int y, int z)
    {
        var pos = new WorldPos();
        float multiple = chunkSize;
        pos.x = Mathf.FloorToInt(x / multiple) * chunkSize;
        pos.y = Mathf.FloorToInt(y / multiple) * chunkSize;
        pos.z = Mathf.FloorToInt(z / multiple) * chunkSize;
        Chunk containerChunk = null;
        chunks.TryGetValue(pos, out containerChunk);

        return containerChunk;
    }

    public Block GetBlock(int x, int y, int z)
    {
        var containerChunk = GetChunk(x, y, z);
        if (containerChunk != null)
        {
            var block = containerChunk.GetBlock(
                x - containerChunk.pos.x,
                y - containerChunk.pos.y,
                z - containerChunk.pos.z);

            return block;
        }

        return new Block(0);
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        var chunk = GetChunk(x, y, z);

        if (chunk != null)
        {
            chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, block);
            chunk.update = true;
        }
    }
    
    public void UnloadChunk(int x, int y, int z)
    {
        Chunk chunk = null;
        if (chunks.TryGetValue(new WorldPos(x, y, z), out chunk))
        {
            Serialization.SaveChunk(chunk);
            Destroy(chunk.gameObject);
            chunks.Remove(new WorldPos(x, y, z));
        }
    }
    
}