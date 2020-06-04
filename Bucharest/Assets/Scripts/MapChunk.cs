/* Author(s): Alexander Waters
 * Updated: 6/3/2020
 * Resources: 
 * https://www.youtube.com/watch?v=64NblGkAabk
 * https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
 * Purpose: Generates A Map for the player to walk on 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TreeEditor;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.UI;
using UnityEngine;

public class MapChunk : MonoBehaviour
{
    

    // properties
    //[SerializeField] private Sprite sourceImg = null;

    [Range(0, 6)]
    [SerializeField] private int levelOfDetail;

    [SerializeField] private int seed = 0;
    // changes random output, needs reworking

    [SerializeField] private int heightScale = 1;
    // how tall the hills go

    [SerializeField] private int octaves = 3;
    // how many layers of details do we want

    [Range(0, 1)]
    [SerializeField] private float persitance = 2;
    // how much each effect affects the world

    [SerializeField] private float effect = 3;
    // how offten an effect accurs in the layer

    [SerializeField] private AnimationCurve heightCurve = null;
    // height modifies strength by height
    // time of 1 represents heights at y of 1
    // setting time of one to value 0 takes the values near y of one and multiplies it by 0


    [SerializeField] private bool[,] landMap;

    [SerializeField] private Vector2 location;

    [SerializeField] private int chunkSize;





    private List<Vector3> vertices;
    // list of verteices for mesh
    
    private List<int> triangles;
    // list of triangle indexes for mesh
    
    //private int imgHeight;
    //private int imgWidth;
    
    private int vertCount;
    // how many vertices are in the mesh

    

    // gets and setters 


    // methods
    Mesh CreateMesh(float[,] NoiseMap, bool[,] landMap,int levelOfDetail)
    {
        // determins the level of detail to render at
        int meshSimpificationIncriment = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

        // freate Vertices Locations
        bool[,] vertArr = FindVertices(this.chunkSize, this.chunkSize, meshSimpificationIncriment);


        // place vertices in world based on vertArr
        int[,] vertIndexes = CreateVertices(vertArr, NoiseMap, meshSimpificationIncriment);
        

        // Create Triangles
        triangles = new List<int>();

        for (int X = 0; X < vertArr.GetLength(0) - meshSimpificationIncriment; X += meshSimpificationIncriment)
        {
            for (int Z = 0; Z < vertArr.GetLength(1) - meshSimpificationIncriment; Z += meshSimpificationIncriment)
            {
                if (landMap[X, Z] == true)
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

    private bool[,] FindVertices(int imgHeight, int imgWidth, int meshSimpificationIncriment)
    {

        // find the location of all vertices baed on image in represntative 2d array
        bool[,] vertTF = new bool[imgHeight + 1, imgWidth + 1];

        // sets all locations to false
        for (int y = 0; y < imgHeight; y++)
        {
            for (int x = 0; x < imgHeight; x++)
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
        for(int y = 0; y < imgHeight; y += meshSimpificationIncriment)
        {
            for (int x = 0; x < imgWidth; x += meshSimpificationIncriment)
            {
                if (landMap[x, y] == true)
                {
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

    public void Generate(bool[,] landMap, Vector2 location,  int octaves, float persitance, float effect, int heightScale, AnimationCurve heightCurve, int levelOfDetail, int seed, int chunkSize)
    {
        
        this.landMap = landMap;
        this.location = location;
        this.octaves = octaves;
        this.persitance = persitance;
        this.effect = effect;
        this.heightCurve = heightCurve;
        this.levelOfDetail = levelOfDetail;
        this.seed = seed;
        this.chunkSize = chunkSize;
        this.heightScale = heightScale;

        //this.imgWidth = sourceImg.texture.width;
        //this.imgHeight = sourceImg.texture.height;

        vertCount = 0;

        float[,] NoiseMap = GenerateNoiseMaps(location);
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


    private float[,] GenerateNoiseMaps(Vector2 landMap)
    {
        // create a item to return 
        float[,] NoiseMap = new float[this.chunkSize+1, this.chunkSize+1];

        // generate map heights with octaves each octave being more imporatant in generation than last
        for (int X = 0; X < this.chunkSize + 1; X++)
        {
            for (int Z = 0; Z < this.chunkSize + 1; Z++)
            {
                
                float amp = 1;
                float freq = 1;

                float noiseHeight = 0;

                for (int oct = 0; oct < this.octaves; oct++)
                {

                    float PerlinX = ((float)X + (this.location.x * chunkSize))  / (float)this.chunkSize * freq;
                    float PerlinZ = ((float)Z + (this.location.y * chunkSize)) / (float)this.chunkSize * freq;

                    float perlinValue = (Mathf.PerlinNoise(this.seed + PerlinX, this.seed + PerlinZ) * 2 - 1) * this.heightScale;
                    perlinValue *= this.heightCurve.Evaluate(perlinValue);

                    noiseHeight += perlinValue * amp;

                    amp *= this.persitance;
                    freq *= this.effect;

                    NoiseMap[X, Z] = noiseHeight;
                }
            }
        }

        return NoiseMap;
    }



    
    
    

}
