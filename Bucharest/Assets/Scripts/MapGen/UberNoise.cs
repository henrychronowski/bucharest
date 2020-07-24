using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Animations;

public class UberNoise
{
    public float[,] CreateNoise(Vector2 location, int[,] landMap, Dictionary<int, BiomeData> biomeLogic, int chunkSize, int seed)
    {

        // create a item to return 
        float[,] NoiseMap = new float[landMap.GetLength(0), landMap.GetLength(1)];


        // generate map heights with octaves each octave being more imporatant in generation than last
        for (int X = 0; X < NoiseMap.GetLength(0); X++)
        {
            for (int Z = 0; Z < NoiseMap.GetLength(1); Z++)
            {
                
                /*
                int biomeColor = landMap[X, Z];
                
                float amp = 1;
                float freq = 1;

                float noiseHeight = 0;

                for (int oct = 0; oct < biomeLogic[biomeColor].octaves; oct++)
                {

                    float PerlinX = ((float)X + (location.x * chunkSize)) / (float)chunkSize * freq;
                    float PerlinZ = ((float)Z + (location.y * chunkSize)) / (float)chunkSize * freq;

                    float perlinValue = (Mathf.PerlinNoise(seed + PerlinX, seed + PerlinZ) * 2 - 1) * biomeLogic[biomeColor].heightScale;
                    perlinValue *= biomeLogic[biomeColor].heightCurve.Evaluate(perlinValue);

                    noiseHeight += perlinValue * amp;

                    amp *= biomeLogic[biomeColor].persitance;
                    freq *= biomeLogic[biomeColor].effect;
                }
                
                */


                float noiseHeight = pattern(new Vector2(X, Z), location);
                
                //Debug.Log(X.ToString() + " " + Z.ToString() + " " + noiseHeight);
                

                NoiseMap[X, Z] = noiseHeight;
                
            }
        }

        return NoiseMap;


    }

    //https://www.iquilezles.org/www/articles/fbm/fbm.htm

    float pattern(in Vector2 p, Vector2 location)
    {

        
        Vector2 q = new Vector2(
            fbm(p + new Vector2(0.0f, 0.0f), 1f, location), 
            fbm(p + new Vector2(5.2f, 1.3f), 1f, location)
        );

        Vector2 r = new Vector2(
            fbm(p + (new Vector2(4.0f, 4.0f) * q) + new Vector2(1.7f, 9.2f), 1f, location),
            fbm(p + (new Vector2(4.0f, 4.0f) * q) + new Vector2(8.3f, 2.8f), 1f, location)
        );


        return fbm( p + new Vector2(40.0f, 40.0f) * q, 1f, location); // ctrl values here

    }



    float fbm(in Vector2 x, in float H, Vector2 location) //ctrl values here
    {
        float height = 0.0f;
        for (int i = 0; i < 3; i++) //ctrl values here
        {
            float freq = Mathf.Pow(2.0f, i); 
            float amp = Mathf.Pow(freq, -H); 
            height += amp * ((Mathf.PerlinNoise(freq * (x.x + (location.x * 240))  / 240, freq * (x.y + (location.y * 240)) / 240) * 2 - 1 ) * 10); //ctrl values here

        }
        return height;
    }

    /*

    float fbm(Vector2 point, float scale = 240, int octaves = 3, float lacunarity = 2, float gain = 0.5f)
    {

        float total = 0;
        float amplitude = 1;
        float frequency = 1;

        for (var i = 0; i < octaves; i++)
        {
            total += (Mathf.PerlinNoise(point.x / scale, point.y / scale) * 2 - 1) * amplitude;
            frequency *= lacunarity;
            amplitude *= gain;
        }

        return total;
    }

    */

}
