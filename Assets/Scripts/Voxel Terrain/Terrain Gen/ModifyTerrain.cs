using UnityEngine;

public class ModifyTerrain : MonoBehaviour
{
    private GameObject _cameraGO;
    private World _world;
    public LayerMask groundMask;

    private void Start()
    {
        _world = gameObject.GetComponent<World>();
        _cameraGO = Camera.main.gameObject;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) ReplaceBlockCursor(new Block(0));

        if (Input.GetMouseButtonDown(1)) AddBlockCursor(new Block(1));
    }

    public void ReplaceBlockCenter(float range, Block block)
    {
        //Replaces the block directly in front of the player

        var ray = new Ray(_cameraGO.transform.position, _cameraGO.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5, groundMask))
            if (hit.distance < range)
                ReplaceBlockAt(hit, block);
    }

    public void AddBlockCenter(float range, Block block)
    {
        //Adds the block specified directly in front of the player

        var ray = new Ray(_cameraGO.transform.position, _cameraGO.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5, groundMask))
        {
            if (hit.distance < range) AddBlockAt(hit, block);

            Debug.DrawLine(ray.origin, ray.origin + ray.direction * hit.distance, Color.green, 2);
        }
    }


    public void ReplaceBlockCursor(Block block)
    {
        //Replaces the block specified where the mouse cursor is pointing

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5, groundMask))
        {
            ReplaceBlockAt(hit, block);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * hit.distance,
                Color.green, 2);
        }
    }

    public void AddBlockCursor(Block block)
    {
        //Adds the block specified where the mouse cursor is pointing

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5, groundMask))
        {
            AddBlockAt(hit, block);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * hit.distance,
                Color.green, 2);
        }
    }

    public void ReplaceBlockAt(RaycastHit hit, Block block)
    {
        //removes a block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
        var position = hit.point;
        position += hit.normal * -0.5f;

        SetBlockAt(position, block);
    }

    public void AddBlockAt(RaycastHit hit, Block block)
    {
        //adds the specified block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
        var position = hit.point;
        position += hit.normal * 0.5f;

        SetBlockAt(position, block);
    }

    public void SetBlockAt(Vector3 position, Block block)
    {
        //sets the specified block at these coordinates

        var x = Mathf.RoundToInt(position.x);
        var y = Mathf.RoundToInt(position.y);
        var z = Mathf.RoundToInt(position.z);

        SetBlockAt(x, y, z, block);
    }

    public void SetBlockAt(int x, int y, int z, Block block)
    {
        //adds the specified block at these coordinates
        _world.SetBlock(x, y, z, block);
        UpdateChunkAt(x, y, z);
    }

    //To do: add a way to just flag the chunk for update then it update it in lateupdate
    public void UpdateChunkAt(int x, int y, int z)
    {
        //Updates the chunk containing this block

        var updateX = Mathf.FloorToInt(x / Chunk.chunkSize);
        var updateY = Mathf.FloorToInt(y / Chunk.chunkSize);
        var updateZ = Mathf.FloorToInt(z / Chunk.chunkSize);

        _world.GetChunk(updateX,updateY,updateZ).update = true;

        //updates neighbour chunks
        if (x - _world.chunkSize * updateX == 0 && updateX != 0)
            _world.GetChunk(updateX - 1,updateY,updateZ).update = true;

        if (x - _world.chunkSize * updateX == 15)
            _world.GetChunk(updateX+1,updateY,updateZ).update = true;

        if (y - _world.chunkSize * updateY == 0 && updateY != 0)
            _world.GetChunk(updateX,updateY-1,updateZ).update = true;

        if (y - _world.chunkSize * updateY == 15)
            _world.GetChunk(updateX,updateY+1,updateZ).update = true;

        if (z - _world.chunkSize * updateZ == 0 && updateZ != 0)
            _world.GetChunk(updateX,updateY,updateZ-1).update = true;

        if (z - _world.chunkSize * updateZ == 15)
            _world.GetChunk(updateX,updateY,updateZ+1).update = true;
    }
    
}