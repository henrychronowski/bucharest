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
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

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


    private List<Vector3> norms;
    // list of Normals for mesh

    private List<Vector3> vertices;
    // list of verteices for mesh
    
    private int vertCount;
    // how many vertices are in the mesh

    

    // gets and setters 


    /*none*/




    // methods

    private Vector3[] CalculateNormals(List<int> triangles)
    {
        Vector3[] vertexNormals = new Vector3[this.vertices.Count];
        int trianglesCount = triangles.Count/3;
        for(int i = 0; i < trianglesCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        for(int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }


        return vertexNormals;
    }



    Mesh CreateMesh(NoiseSample[,] NoiseMap, int[,] landMap, int levelOfDetail)
    {
        // determins the level of detail to render at
        int meshSimpificationIncriment = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

        // create Vertices Locations
        bool[,] vertArr = FindVertices(landMap, meshSimpificationIncriment);

        // place vertices in world based on vertArr
        int[,] vertIndexes = CreateVertices(vertArr, NoiseMap, meshSimpificationIncriment);

        // Create Triangles
        List<int> triangles = CreateTriangles(vertArr, vertIndexes, meshSimpificationIncriment);

        
        


        // Create Mesh
        Mesh mesh = new Mesh();
        mesh.vertices = this.vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        //create UV
        mesh.uv = getUVs(vertIndexes, vertArr, meshSimpificationIncriment);

        //math that needs to change to dislove seames
        mesh.normals = CalculateNormals(triangles);

        return mesh;
    }

    private Vector2[] getUVs(int[,] vertsIndex, bool[,] vertArr, int meshSimpificationIncriment)
    {

        List<Vector2> uv = new List<Vector2>();

        for (int y = 0; y < vertsIndex.GetLength(1); y += meshSimpificationIncriment)
        {
            for (int x = 0; x < vertsIndex.GetLength(0); x += meshSimpificationIncriment)
            {
                if (vertArr[x, y])
                {
                    uv.Add(new Vector2(x / (float)vertsIndex.GetLength(0), y / (float)vertsIndex.GetLength(1)));
                }
            }
        }

        return uv.ToArray();
    }

    private List<int> CreateTriangles(bool[,] vertArr, int[,] vertIndexes, int meshSimpificationIncriment)
    {

        Debug.Log("VertArr: " + vertArr.GetLength(0) + "    LandMap: " + landMap.GetLength(0) + "    VertIndexes: " + vertIndexes.GetLength(0));

        // kill me
        List<int> triangles = new List<int>();

        for (int X = 0; X < landMap.GetLength(0) - meshSimpificationIncriment - 2; X += meshSimpificationIncriment)
        {
            for (int Z = 0; Z < landMap.GetLength(1) - meshSimpificationIncriment - 2; Z += meshSimpificationIncriment)
            {
                if (landMap[X+1, Z+1] != 0)
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

        return triangles;

    }


    private int[,] CreateVertices(bool[,] vertArr, NoiseSample[,] noiseMap, int meshSimpificationIncriment)
    {
        // vert index tells us the vertsindex in the vertices list
        // needed for mesh gen

        Debug.Log("NoiseMap: " + noiseMap.GetLength(0) + "   VertArr: " +  vertArr.GetLength(0));

        this.vertices = new List<Vector3>();
        this.norms = new List<Vector3>();
        
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
        for (int y = 0; y < noiseMap.GetLength(1) - 2; y += meshSimpificationIncriment)
        {
            for (int x = 0; x < noiseMap.GetLength(0) - 2; x += meshSimpificationIncriment)
            {
                if (vertArr[x,y])
                {
                    this.vertices.Add(new Vector3(x, noiseMap[x + 1, y + 1].value, y));

                    vertsIndex[x, y] = this.vertCount;

                    this.vertCount++;





                    

                   //used to recalcuate normals TO DO
                    /*
                    Vector3 derivsAvg = new Vector3(0, 0, 0);
                    
                    
                    for (int l = -1; l < 2; l++)
                    {
                        for(int m = -1; m < 2; m++)
                        {
                            derivsAvg += noiseMap[x + 1 + l, y + 1+ m].derivative;
                        }
                    }
                    derivsAvg = (derivsAvg / 9);

                    this.norms.Add(new Vector3(-derivsAvg.x, 1f, -derivsAvg.y).normalized);
                    */
                    
                }
            }
        }

        return vertsIndex;

    }






    private bool[,] FindVertices(int[,] landMap, int meshSimpificationIncriment)
    {

       
        // find the location of all vertices baed on image in represntative 2d array
        bool[,] vertTF = new bool[landMap.GetLength(0) - 2, landMap.GetLength(0) - 2];

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
        for(int y = 0; y < landMap.GetLength(1) - meshSimpificationIncriment - 2; y += meshSimpificationIncriment)
        {
            for (int x = 0; x < landMap.GetLength(0) - meshSimpificationIncriment - 2; x += meshSimpificationIncriment)
            {
                if (landMap[x + 1, y + 1] != 0)
                {
                    for(int k = 0; k < 4; k++)
                    {
                        vertTF[x + placesToAdd[k, 0], y + placesToAdd[k, 1]] = true;   
                    }
                }
                
            }
        }

        Debug.Log("Land Map: " + landMap.GetLength(0) + "   VertTf: " + vertTF.GetLength(0));
        return vertTF;
        
    }


    public void Generate(int[,] landMap, Vector2 location, Dictionary<int, BiomeData> biomeLogic, int lod, int seed, int chunkSize)
    {
        
        this.landMap = landMap;
        this.location = location;
        this.levelOfDetail = lod;
        this.seed = seed;
        this.chunkSize = chunkSize;

        this.vertCount = 0;
        
        NoiseSample[,] NoiseMap = GenerateNoiseMaps(location, landMap, biomeLogic);
        
        Mesh finalMesh = CreateMesh(NoiseMap, this.landMap, this.levelOfDetail);


        GetComponent<MeshFilter>().mesh = finalMesh;
        GetComponent<MeshCollider>().sharedMesh = finalMesh;
    }




    private NoiseSample[,] GenerateNoiseMaps(Vector2 location, int[,] landMap, Dictionary<int, BiomeData> biomeLogic)
    {
        UberNoise Noise = new UberNoise();
        return Noise.CreateNoise(location, landMap, biomeLogic, this.chunkSize, this.seed);
    }



    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = this.vertices[indexA];
        Vector3 pointB = this.vertices[indexB];
        Vector3 pointC = this.vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        return Vector3.Cross(sideAB, sideAC).normalized;


    }

    /*
   private void OnDrawGizmos()
   {


           RaycastHit hit;

           // send ray cast to center screen
           if (!Physics.Raycast(SceneView.currentDrawingSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out hit))
               return;

           // make sure we hit some mesh
           MeshCollider meshCollider = hit.collider as MeshCollider;
           if (meshCollider == null || meshCollider.sharedMesh == null)
               return;

           // get containers set up
           Mesh mesh = meshCollider.sharedMesh;
           Vector3[] vertices = mesh.vertices;
           int[] triangles = mesh.triangles;

           List<Vector3> showPOS = new List<Vector3>();
           List<int> showPOSindex = new List<int>();



           for (int i = 0; i < 3; i++)
           {
               //init triangle
               showPOS.Add(vertices[triangles[hit.triangleIndex * 3 + i]]);
               showPOSindex.Add(triangles[hit.triangleIndex * 3 + i]);

               showPOS.Add(vertices[triangles[hit.triangleIndex * 3 + i]]);
               showPOSindex.Add(triangles[hit.triangleIndex * 3 + i]);

           }





           // get location that was hit
           Transform hitTransform = hit.collider.transform;


           // draw spehere, text, and alighn point
           for (int i = 0; i < showPOS.Count; i++)
           {
               showPOS[i] = hitTransform.TransformPoint(showPOS[i]);
               Gizmos.DrawSphere(showPOS[i], .1f);
               Handles.Label(showPOS[i], (showPOS[i].x + "-" + showPOS[i].z + "-" + showPOSindex[i]));
           }


           // draw lines
           for (int i = 0; i < showPOS.Count; i += 3)
           {
               Gizmos.DrawLine(showPOS[i], showPOS[i + 1]);
               Gizmos.DrawLine(showPOS[i + 1], showPOS[i + 2]);
               Gizmos.DrawLine(showPOS[i + 2], showPOS[i]);

           }


       
    }*/


}




