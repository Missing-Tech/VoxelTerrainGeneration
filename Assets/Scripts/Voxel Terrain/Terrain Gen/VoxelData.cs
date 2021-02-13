using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelData
{
    public int GetCell(int x, int y, int z, Chunk chunk)
    {
        return chunk.GetBlock(x, y, z).value;
    }

    public int GetNeighbour(int x, int y, int z, int chunkX, int chunkY, int chunkZ, Direction dir, Chunk chunk)
    {
        DataCoordinate offsetToCheck = _offsets[(int) dir];
        DataCoordinate neighbourCoord =
            new DataCoordinate(x + offsetToCheck.x, y + offsetToCheck.y, z + offsetToCheck.z);

        if (neighbourCoord.x < 0 || neighbourCoord.x >= Chunk.chunkSize
                                 || neighbourCoord.y < 0 || neighbourCoord.y >= Chunk.chunkSize
                                 || neighbourCoord.z < 0 || neighbourCoord.z >= Chunk.chunkSize)
        {
            return 0;
        }

        return GetCell(neighbourCoord.x, neighbourCoord.y, neighbourCoord.z, chunk);
    }

    struct DataCoordinate
    {
        public int x;
        public int y;
        public int z;

        public DataCoordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    private readonly DataCoordinate[] _offsets =
    {
        new DataCoordinate(0, 0, 1),
        new DataCoordinate(1, 0, 0),
        new DataCoordinate(0, 0, -1),
        new DataCoordinate(-1, 0, 0),
        new DataCoordinate(0, 1, 0),
        new DataCoordinate(0, -1, 0),
    };
}

public enum Direction
{
    North,
    East,
    South,
    West,
    Up,
    Down,
}