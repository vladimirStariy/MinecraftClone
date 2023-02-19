using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public float BaseHeight = 8;
    public NoiseOctaveSettings[] Octaves;

    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }

    private FastNoiseLite[] octaveNoises;

    public void Awake() 
    {
        octaveNoises = new FastNoiseLite[Octaves.Length];
        for(int i = 0; i < Octaves.Length; i++)
        {
            octaveNoises[i] = new FastNoiseLite();
            octaveNoises[i].SetNoiseType(Octaves[i].NoiseType);
            octaveNoises[i].SetFrequency(Octaves[i].Frequency);
        }
    }

    public BlockEnum[,,] GenerateTerrain(float xOffset, float zOffset)
    {
        var result = new BlockEnum[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];
        
        for (int x = 0; x < ChunkRenderer.ChunkWidth; x++)
        {
            for (int z = 0; z < ChunkRenderer.ChunkWidth; z++)
            {
                float height = GetHeight(x * ChunkRenderer.blockScale + xOffset, z * ChunkRenderer.blockScale + zOffset);
                float dirtHeight = 1; 
                
                for (int y = 0; y < height / ChunkRenderer.blockScale; y++)
                {
                    if(height - y*ChunkRenderer.blockScale < dirtHeight)
                    {
                        result[x,y,z] = BlockEnum.Grass;
                    }
                    else
                    {
                        result[x,y,z] = BlockEnum.Stone;
                    }
                }
            }
        }
        return result;
    }

    private float GetHeight(float x, float y)
    {
        float result = BaseHeight;
        for(int i = 0; i < Octaves.Length; i++)
        {
            float noise = octaveNoises[i].GetNoise(x, y);
            result += noise * Octaves[i].Amplitude / 2;
        }

        return result;
    }
}
