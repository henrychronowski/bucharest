using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements.Experimental;

public class UberNoise
{
    public NoiseSample[,] CreateNoise(Vector2 location, int[,] landMap, Dictionary<int, BiomeData> biomeLogic, int chunkSize, int seed)
    {

        // create a item to return 
        NoiseSample[,] NoiseMap = new NoiseSample[landMap.GetLength(0), landMap.GetLength(1)];


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

                Vector3 pos = (Vector3)((new Vector2(X, Z) + location * 240f) / 240f);


                NoiseSample height = findNoise(pos);


                NoiseMap[X, Z] = height;
                
            }
        }

        return NoiseMap;


    }






    NoiseSample findNoise(Vector3 inital)
    {

        
        //Mountins
        Vector3 q = new Vector2(
            Sum(inital + new Vector3(0, 0), 1f, 1, 1, 0.5f).value,
            Sum(inital + new Vector3(1.2f, 1.3f), 1f, 1, 1, 0.5f).value
        );

        //Vector3 r = new Vector2(
        //    Sum(inital + (q) + new Vector3(0, 0), 1f, 1, 1, 0.5f).value,
        //    Sum(inital + (q) + new Vector3(5.2f, 1.3f), 1f, 1, 1, 0.5f).value
        //);
        
        NoiseSample Mountians = Sum(inital + q, 1f, 2, 2f, 0.5f);


        //rubble





        float rubble = Sum(Vector3.Scale((inital + q), new Vector3(1, 1, 400)), 10, 2, 10, 0.7f).value;
        //float hilly = hills(pos);

        //float rub = rubble(point);

        //Debug.Log(Mountians.derivative.x + " " + Mountians.derivative.y + " " + Mountians.derivative.z);

        float deriv = Vector3.Dot(Mountians.derivative, Mountians.derivative);

        return Mountians * 100;

        //return (rubble * (1f-sigmoid(deriv))) + (1 - Mathf.Abs(Mountians.value)) * 100;
    }
    








    float sigmoid(float val)
    {
        return (float)(1.0f / (1.0f + Mathf.Exp(-val)));
    }


    float hills(in Vector2 p)
    {

        return Mathf.Abs(Sum(p,10,2,1,0.5f).value);

    }

    //modified code from https://www.iquilezles.org/www/articles/fbm/fbm.htm

    float moutins(in Vector2 p)
    {



        Vector2 q = new Vector2(
            Sum(p + new Vector2(0, 0), 1f, 1, 1, 0.5f).value,
            Sum(p + new Vector2(5.2f, 1.3f), 1f, 1, 1, 0.5f).value
        );



        Vector2 r = new Vector2(
            Sum(p + (2 * q) + new Vector2(0, 0), 1f, 1, 1, 0.5f).value,
            Sum(p + (2 * q) + new Vector2(5.2f, 1.3f), 1f, 1, 1, 0.5f).value
        );

        return 1 - Mathf.Abs(Sum(p + 2 * r, 0.5f, 2, 1, 0.2f).value ) ;





    }



    

    float rubble(in Vector2 p)
    {
        /*
        Vector2 q = new Vector2(
            Sum(p + new Vector2(0.0f, 0.0f)).value,
            Sum(p + new Vector2(5.2f, 1.3f)).value
        );

        Vector2 r = new Vector2(
            Sum(p + (4.0f * q) + new Vector2(1.7f, 9.2f)).value,
            Sum(p + (4.0f * q) + new Vector2(8.3f, 2.8f)).value
        );
        */

        NoiseSample result = Sum(p, 10,5,20,0.7f);
        float final = result.value * 1-sigmoid(result.derivative.magnitude);

        return final; // ctrl values here


    }


    //https://www.youtube.com/watch?v=C9RyEiEzMiU&feature=youtu.be&t=1958
    float Customfbm(Vector2 x, Vector2 location, int octs = 5, float lacunarity = 1, float gain = 1)
    {
        float freq = 1, amp = 1;
        float sum = 0;
        for(int Octave = 0; Octave < octs; Octave++)
        {
            float value = Mathf.PerlinNoise(freq * (x.x + (location.x * 240)) / 240, freq * (x.y + (location.y * 240)) / 240) * 10;
            sum += value * amp;
            freq *= lacunarity;
            amp *= gain;
        }
        return sum;
    }


    //https://catlikecoding.com/unity/tutorials/noise-derivatives/

    private static float Smooth(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    private static float SmoothDerivative(float t)
    {
        return 30f * t * t * (t * (t - 2f) + 1f);
    }

    private static int[] hash = {
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
    };

    private const int hashMask = 255;

    private static Vector2[] gradients2D = {
        new Vector2( 1f, 0f),
        new Vector2(-1f, 0f),
        new Vector2( 0f, 1f),
        new Vector2( 0f,-1f),
        new Vector2( 1f, 1f).normalized,
        new Vector2(-1f, 1f).normalized,
        new Vector2( 1f,-1f).normalized,
        new Vector2(-1f,-1f).normalized
    };

    private const int gradientsMask2D = 7;

    private static float Dot(Vector2 g, float x, float y)
    {
        return g.x * x + g.y * y;
    }


    private static float sqr2 = Mathf.Sqrt(2f);

    public static NoiseSample Perlin2D(Vector3 point, float frequency)
    {

        point *= frequency;
        int ix0 = Mathf.FloorToInt(point.x);
        int iy0 = Mathf.FloorToInt(point.y);
        float tx0 = point.x - ix0;
        float ty0 = point.y - iy0;
        float tx1 = tx0 - 1f;
        float ty1 = ty0 - 1f;
        ix0 &= hashMask;
        iy0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;

        int h0 = hash[ix0];
        int h1 = hash[ix1];
        Vector2 g00 = gradients2D[hash[h0 + iy0] & gradientsMask2D];
        Vector2 g10 = gradients2D[hash[h1 + iy0] & gradientsMask2D];
        Vector2 g01 = gradients2D[hash[h0 + iy1] & gradientsMask2D];
        Vector2 g11 = gradients2D[hash[h1 + iy1] & gradientsMask2D];

        float v00 = Dot(g00, tx0, ty0);
        float v10 = Dot(g10, tx1, ty0);
        float v01 = Dot(g01, tx0, ty1);
        float v11 = Dot(g11, tx1, ty1);

        float dtx = SmoothDerivative(tx0);
        float dty = SmoothDerivative(ty0);
        float tx = Smooth(tx0);
        float ty = Smooth(ty0);

        float a = v00;
        float b = v10 - v00;
        float c = v01 - v00;
        float d = v11 - v01 - v10 + v00;

        Vector2 da = g00;
        Vector2 db = g10 - g00;
        Vector2 dc = g01 - g00;
        Vector2 dd = g11 - g01 - g10 + g00;

        NoiseSample sample;
        sample.value = a + b * tx + (c + d * tx) * ty;
        sample.derivative = da + db * tx + (dc + dd * tx) * ty;
        sample.derivative.x += (b + d * ty) * dtx;
        sample.derivative.y += (c + d * tx) * dty;
        sample.derivative.z = 0f;
        sample.derivative *= frequency;
        return sample * sqr2;
    }

    public static NoiseSample Sum(Vector3 point, float frequency = 1, int octaves = 2, float lacunarity = 1, float persistence = 0.5f)
    {
        NoiseSample sum = Perlin2D(point, frequency);
        float amplitude = 1f;
        float range = 1f;
        for (int o = 1; o < octaves; o++)
        {
            frequency *= lacunarity;
            amplitude *= persistence;
            range += amplitude;
            sum += Perlin2D(point, frequency) * amplitude;
        }
        return sum * (1f / range); 
    }

}
