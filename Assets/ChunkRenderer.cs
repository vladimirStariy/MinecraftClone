using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 15;
    public const int ChunkHeight = 128;
    public const float blockScale = 1;

    public ChunkData chunkData;

    public BlockInfo[] Blocks;

    public WorldScript parentWorld;

    private Mesh chunkMesh;

    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> UVs = new List<Vector2>();
    private List<int> triangles = new List<int>();

    void Start()
    {
        chunkMesh = new Mesh();

        RegenerateMesh();

        GetComponent<MeshFilter>().mesh = chunkMesh;
    }

    public void PlaceBlock(Vector3Int blockPosition)
    {
        chunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockEnum.Stone;
        RegenerateMesh();
    }

    public void DestroyBlock(Vector3Int blockPosition)
    {
        chunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockEnum.Air;
        RegenerateMesh();
    }

    public void RegenerateMesh()
    {
        verticies.Clear();
        triangles.Clear();
        UVs.Clear();
        for(int y = 0; y < ChunkHeight; y++)
        {
            for(int x = 0; x < ChunkWidth; x++)
            {
                for(int z = 0; z < ChunkWidth; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        chunkMesh.triangles = Array.Empty<int>();
        chunkMesh.vertices = verticies.ToArray();
        chunkMesh.uv = UVs.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();

        GetComponent<MeshCollider>().sharedMesh = chunkMesh; 
    }

    private void GenerateBlock(int x, int y, int z)
    {
        var blockPosition = new Vector3Int(x,y,z);
        var blockType = GetBlockAtPosition(blockPosition);
        if(blockType == BlockEnum.Air) return;

        if(GetBlockAtPosition(blockPosition + Vector3Int.right) == 0)
        {
            GenerateRightSide(blockPosition);
            AddUVs(blockType);
        }    
        if(GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)
        {
            GenerateLeftSide(blockPosition);
            AddUVs(blockType);
        }
        if(GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(blockPosition);
            AddUVs(blockType);
        }
        if(GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)  
        {
            GenerateBackSide(blockPosition);
            AddUVs(blockType);
        }  
        if(GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)    
        {
            GenerateTopSide(blockPosition);
            AddUVs(blockType);
        }   
        if(GetBlockAtPosition(blockPosition + Vector3Int.down) == 0) 
        {
            GenerateBottomSide(blockPosition);
            AddUVs(blockType);
        }
    }

    private BlockEnum GetBlockAtPosition(Vector3Int blockPosition)
    {
        if(blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
           blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
           blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            return chunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            if(blockPosition.y < 0 || blockPosition.y >= ChunkWidth) 
                return BlockEnum.Air;
            Vector2Int adjacentChunkPosition = chunkData.ChunkPosition;
            if(blockPosition.x < 0)
            {
                adjacentChunkPosition.x--;
                blockPosition.x += ChunkWidth;
            }
            else if(blockPosition.x >= ChunkWidth)
            {
                adjacentChunkPosition.x++;
                blockPosition.x -= ChunkWidth;
            }
            if(blockPosition.z < 0)
            {
                adjacentChunkPosition.y--;
                blockPosition.z += ChunkWidth;
            }
            else if(blockPosition.z >= ChunkWidth)
            {
                adjacentChunkPosition.y++;
                blockPosition.z -= ChunkWidth;
            }

            if(parentWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }
            else
            {
                return BlockEnum.Air;
            } 
        }
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(1,0,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,1,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,0,1) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,1,1) + blockPosition)*blockScale);

        AddLastVerticies();
    }

    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0,0,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(0,0,1) + blockPosition)*blockScale);
        verticies.Add((new Vector3(0,1,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(0,1,1) + blockPosition)*blockScale);
        AddLastVerticies();
    }

    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0,0,1) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,0,1) + blockPosition)*blockScale);
        verticies.Add((new Vector3(0,1,1) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,1,1) + blockPosition)*blockScale);
        AddLastVerticies();
    }

    private void GenerateBackSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0,0,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(0,1,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,0,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,1,0) + blockPosition)*blockScale);

        AddLastVerticies();
    }

    private void GenerateTopSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0,1,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(0,1,1) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,1,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,1,1) + blockPosition)*blockScale);

        AddLastVerticies();
    }

    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0,0,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,0,0) + blockPosition)*blockScale);
        verticies.Add((new Vector3(0,0,1) + blockPosition)*blockScale);
        verticies.Add((new Vector3(1,0,1) + blockPosition)*blockScale);

        AddLastVerticies();
    }

    private void AddLastVerticies()
    {
        triangles.Add(verticies.Count - 4);
        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 2);

        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 1);
        triangles.Add(verticies.Count - 2);
    }

    private void AddUVs(BlockEnum blockType)
    {
        Vector2 uv = new Vector2();
        BlockInfo info = Blocks.FirstOrDefault(b => b.BlockType == blockType);

        if(info != null)
        {
            uv = new Vector2(info.PixelsOffsetX.x, info.PixelsOffsetX.y);
        }

        UVs.Add(new Vector2(uv.x / 48, uv.y / 16));
        UVs.Add(new Vector2(uv.x / 48, 1));
        UVs.Add(new Vector2((uv.x + 16) / 48, uv.y / 16));
        UVs.Add(new Vector2((uv.x + 16) / 48, 1));
    }

}
