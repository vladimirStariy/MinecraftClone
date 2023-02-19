using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScript : MonoBehaviour
{
    private const int RenderRadius = 5;
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
    public ChunkRenderer ChunkPrefab;
    private Vector2Int currentPlayerChunk;
    public TerrainGenerator Generator;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        Generate();
    }

    private void Generate()
    {
        for(int x = 0; x < 10; x++)
        {
            for(int y = 0; y < 10; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                if(ChunkDatas.ContainsKey(new Vector2Int(x,y))) continue;

                float xPos = x * ChunkRenderer.ChunkWidth * ChunkRenderer.blockScale;
                float zPos = y * ChunkRenderer.ChunkWidth * ChunkRenderer.blockScale;

                ChunkData chunkData = new ChunkData();
                chunkData.ChunkPosition = new Vector2Int(x, y);
                chunkData.Blocks = Generator.GenerateTerrain(xPos, zPos);
                ChunkDatas.Add(new Vector2Int(x, y), chunkData);

                var chunk = Instantiate(ChunkPrefab, new Vector3(xPos,0,zPos), Quaternion.identity, transform);
                chunk.chunkData = chunkData;
                chunk.parentWorld = this;

                chunkData.renderer = chunk;
            }
        }
    }

    private void LoadChunkAt(Vector2Int chunkPosition)
    {

    }

    void Update()
    {

    }

    public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
    {
        return new Vector2Int(blockWorldPos.x / ChunkRenderer.ChunkWidth, blockWorldPos.z / ChunkRenderer.ChunkWidth);
    }
}
