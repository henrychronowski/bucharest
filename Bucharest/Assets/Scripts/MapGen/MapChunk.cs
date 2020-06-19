﻿/* Author(s): Alexander Waters
 * Updated: 6/3/2020
 * Resources: 
 * https://www.youtube.com/watch?v=64NblGkAabk
 * https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
 * Purpose: Generates A Map for the player to walk on 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapChunk : MonoBehaviour
{
    


    [SerializeField] private int seed = 0;
    // changes random output, needs reworking

    [SerializeField] private int[,] landMap;
    //a 2d map dpeciting biomes 


    [SerializeField] private Vector2 location;
    //where it is in realtion to the whole map based on chunck location


    [SerializeField] private int chunkSize;
    //how big each chunk is 
    // should be 240


    [Range(0, 6)]
    [SerializeField] private int levelOfDetail;
    //how detailed the mesh is




    private List<Vector3> vertices;
    // list of verteices for mesh
    
    private List<int> triangles;
    // list of triangle indexes for mesh
    
    private int vertCount;
    // how many vertices are in the mesh

    

    // gets and setters 


    // methods
    Mesh CreateMesh(float[,] NoiseMap, int[,] landMap, int levelOfDetail)
    {
        // determins the level of detail to render at
        int meshSimpificationIncriment = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

        // create Vertices Locations
        bool[,] vertArr = FindVertices(landMap, meshSimpificationIncriment);


        // place vertices in world based on vertArr
        int[,] vertIndexes = CreateVertices(vertArr, NoiseMap, meshSimpificationIncriment);
        

        // Create Triangles
        triangles = new List<int>();

        for (int X = 0; X < vertArr.GetLength(0) - meshSimpificationIncriment; X += meshSimpificationIncriment)
        {
            for (int Z = 0; Z < vertArr.GetLength(1) - meshSimpificationIncriment; Z += meshSimpificationIncriment)
            {
                if (landMap[X, Z] != 0) 
                {
                    triangles.Add(vertIndexes[X, Z]);
                    triangles.Add(vertIndexes[X, Z + meshSimpificationIncriment]);
                    triangles.Add(vertIndexes[X + meshSimpificationIncriment, Z]);

                    triangles.Add(vertIndexes[X, Z + meshSimpificationIncriment]);
                    triangles.Add(vertIndexes[X + meshSimpificationIncriment, Z + meshSimpificationIncriment]);
                    triangles.Add(vertIndexes[X + meshSimpificationIncriment, Z]);

                    
                }

            }
        }

        // Create Mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

        return mesh;
    }

    private int[,] CreateVertices(bool[,] vertArr, float[,] noiseMap, int meshSimpificationIncriment)
    {
        // vert index tells us the vertsindex in the vertices list
        // needed for mesh gen
        this.vertices = new List<Vector3>();
        
        int[,] vertsIndex = new int[vertArr.GetLength(0), vertArr.GetLength(1)];


        


        // sets all values in vertIndex to -1
        for (int x = 0; x < vertsIndex.GetLength(0); x++)
        {
            for (int y = 0; y < vertsIndex.GetLength(1); y++)
            {
                vertsIndex[x, y] = -1;
            }
        }
        

        // place vertices in world and array
        for (int y = 0; y < vertsIndex.GetLength(1); y += meshSimpificationIncriment)
        {
            for (int x = 0; x < vertsIndex.GetLength(0); x += meshSimpificationIncriment)
            {
                if (vertArr[x,y])
                {
                    this.vertices.Add(new Vector3(x, noiseMap[x, y], y));
                    vertsIndex[x, y] = this.vertCount;
                    this.vertCount++;
                }
            }
        }

        return vertsIndex;

    }

    private bool[,] FindVertices(int[,] landMap, int meshSimpificationIncriment)
    {

        // find the location of all vertices baed on image in represntative 2d array
        bool[,] vertTF = new bool[landMap.GetLength(0) - 1, landMap.GetLength(0) - 1];

        // sets all locations to false
        for (int y = 0; y < vertTF.GetLength(1); y++)
        {
            for (int x = 0; x < vertTF.GetLength(0); x++)
            {
                vertTF[x, y] = false;
            }
        }

        // all the places to check made iteratable
        int[,] placesToAdd = new int[4, 2]
        {
            {0, 0},
            {meshSimpificationIncriment, 0},
            {meshSimpificationIncriment, meshSimpificationIncriment},
            {0, meshSimpificationIncriment},
        };


        // finds all the places that need vertices and saves locations to representaive 2d array
        for(int y = 0; y < vertTF.GetLength(1) - meshSimpificationIncriment; y += meshSimpificationIncriment)
        {
            for (int x = 0; x < vertTF.GetLength(0) - meshSimpificationIncriment; x += meshSimpificationIncriment)
            {
                if (landMap[x, y] != 0)
                {
                    //Debug.Log(landMap[x, y].r + " " + landMap[x, y].g + " " + landMap[x, y].b);
                    for(int k = 0; k < 4; k++)
                    {
                        vertTF[x + placesToAdd[k, 0], y + placesToAdd[k, 1]] = true;   
                    }
                }
                
            }
        }

        return vertTF;

    }


    public void Start()
    {
        Generate();
    }

    public void Generate(int[,] landMap, Vector2 location, Dictionary<int, BiomeData> biomeLogic, int lod, int seed, int chunkSize)
    {
        
        this.landMap = landMap;
        this.location = location;
        this.levelOfDetail = lod;
        this.seed = seed;
        this.chunkSize = chunkSize;
        

        //this.imgWidth = sourceImg.texture.width;
        //this.imgHeight = sourceImg.texture.height;

        vertCount = 0;
        
        float[,] NoiseMap = GenerateNoiseMaps(location, landMap, biomeLogic);
        
        Mesh finalMesh = CreateMesh(NoiseMap, this.landMap, this.levelOfDetail);



        GetComponent<MeshFilter>().mesh = finalMesh;
        GetComponent<MeshCollider>().sharedMesh = finalMesh;
    }

    public void Generate()
    {
        //this.imgWidth = sourceImg.texture.width;
        //this.imgHeight = sourceImg.texture.height;

        vertCount = 0;

        //float[,] NoiseMap = GenerateNoiseMaps(this.imgWidth, this.imgHeight);
        //Mesh finalMesh = CreateMesh(NoiseMap, this.levelOfDetail);
        //GetComponent<MeshFilter>().mesh = finalMesh;
        //GetComponent<MeshCollider>().sharedMesh = finalMesh;
    }


    private float[,] GenerateNoiseMaps(Vector2 location, int[,] landMap, Dictionary<int, BiomeData> biomeLogic)
    {
        // create a item to return 
        float[,] NoiseMap = new float[landMap.GetLength(0), landMap.GetLength(1)];


        // generate map heights with octaves each octave being more imporatant in generation than last
        for (int X = 0; X < NoiseMap.GetLength(0); X++)
        {
            for (int Z = 0; Z < NoiseMap.GetLength(1); Z++)
            {

                int biomeColor = landMap[X, Z];

                float amp = 1;
                float freq = 1;

                float noiseHeight = 0;

                for (int oct = 0; oct < biomeLogic[biomeColor].octaves; oct++)
                {

                    float PerlinX = ((float)X + (this.location.x * chunkSize))  / (float)this.chunkSize * freq;
                    float PerlinZ = ((float)Z + (this.location.y * chunkSize)) / (float)this.chunkSize * freq;

                    float perlinValue = (Mathf.PerlinNoise(this.seed + PerlinX, this.seed + PerlinZ) * 2 - 1) * biomeLogic[biomeColor].heightScale;
                    perlinValue *= biomeLogic[biomeColor].heightCurve.Evaluate(perlinValue);

                    noiseHeight += perlinValue * amp;

                    amp *= biomeLogic[biomeColor].persitance;
                    freq *= biomeLogic[biomeColor].effect;
                }

                NoiseMap[X, Z] = noiseHeight;
            }
        }

        return NoiseMap;
    }



    bool RGBequal(Color c1, Color c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b;
    }
}




