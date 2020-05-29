using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public static class Noise
{
    public static float[,] generateNoiseMap(int mapWidth, int mapHeight, float scale, int seed ,int octaves, float persitance, float lacuarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for(int i=0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }


        if (scale <= 0)
        {
            scale = 0.0000f;
        } 

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;



        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float amp = 1;
                float freq = 1;

                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++)
                {
                    float sampleX = (x-halfWidth) / scale * freq + octaveOffsets[i].x;
                    float sampleY = (y-halfHeight) / scale * freq + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;


                    noiseMap[x, y] = perlinValue;

                    noiseHeight += perlinValue * amp;

                    amp *= persitance;
                    freq *= lacuarity;
                }


                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                   minNoiseHeight = noiseHeight;
                }
                
                noiseMap[x, y] = noiseHeight;

            }
        }


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
       return noiseMap;
    }
}
