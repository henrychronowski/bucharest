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

public class MapGen : MonoBehaviour
{
    // properties
    [SerializeField] private Sprite sourceImg = null;
    

    [SerializeField] private int seed = 0;
    // changes random output, needs reworking

    [SerializeField] private int heightScale = 1;
    // how tall the hills go

    [SerializeField] private int octaves = 3;
    // how many layers of details do we want

    [SerializeField] private float persitance = 2;
    // how much each effect affects the world

    [SerializeField] private float effect = 3;
    // how offten an effect accurs in the layer

    [SerializeField] private AnimationCurve heightCurve = null;
    // height modifies strength by height
    // time of 1 represents heights at y of 1
    // setting time of one to value 0 takes the values near y of one and multiplies it by 0

    [SerializeField] private bool autoUpdate = false;
    // will automatily run code 

    [SerializeField] private bool debugTools = false;
    // debugging tools for devs


    private List<Vector3> vertices;
    // list of verteices for mesh
    
    private List<int> triangles;
    // list of triangle indexes for mesh
    
    private int imgHeight;
    private int imgWidth;
    
    private int vertCount;
    // how many vertices are in the mesh

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // gets and setters 
    public bool GetAutoUpdate()
    {
        return this.autoUpdate;
    }

    // methods
    Mesh CreateMesh(float[,] NoiseMap)
    {

        // freate Vertices Locations
        bool[,] vertArr = FindVertices(this.imgHeight, this.imgWidth);


        // place vertices in world based on vertArr
        int[,] vertIndexes = CreateVertices(vertArr, NoiseMap);
        

        // Create Triangles
        triangles = new List<int>();

        for (int X = 0; X < vertArr.GetLength(0) - 1; X++)
        {
            for (int Z = 0; Z < vertArr.GetLength(1) - 1; Z++)
            {
                if (sourceImg.texture.GetPixel(X, Z).grayscale > 0)
                {
                    triangles.Add(vertIndexes[X, Z]);
                    triangles.Add(vertIndexes[X, Z + 1]);
                    triangles.Add(vertIndexes[X + 1, Z]);

                    triangles.Add(vertIndexes[X, Z + 1]);
                    triangles.Add(vertIndexes[X + 1, Z + 1]);
                    triangles.Add(vertIndexes[X + 1, Z]);
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

    private int[,] CreateVertices(bool[,] vertArr, float[,] noiseMap)
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
        for (int y = 0; y < vertArr.GetLength(1); y++)
        {
            for (int x = 0; x < vertArr.GetLength(0); x++)
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

    public bool[,] FindVertices(int imgHeight, int imgWidth)
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
            {1, 0},
            {1, 1},
            {0, 1},
        };


        // finds all the places that need vertices and saves locations to representaive 2d array
        for(int y = 0; y < imgHeight; y++)
        {
            for (int x = 0; x < imgWidth; x++)
            {
                if (sourceImg.texture.GetPixel(x, y).grayscale > 0)
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


    public void Generate()
    {

        imgHeight = sourceImg.texture.height;
        imgWidth = sourceImg.texture.width;
        vertCount = 0;

        float[,] NoiseMap = GenerateNoiseMaps(this.imgWidth, this.imgHeight);
        Mesh finalMesh = CreateMesh(NoiseMap);
        GetComponent<MeshFilter>().mesh = finalMesh;
        GetComponent<MeshCollider>().sharedMesh = finalMesh;
    }


    float[,] GenerateNoiseMaps(int mapWidth, int mapHeight)
    {
        // create a item to return 
        float[,] NoiseMap = new float[mapWidth + 1, mapHeight + 1];

        // generate map heights with octaves each octave being more imporatant in generation than last
        for (int X = 0; X < mapWidth + 1; X++)
        {
            for (int Z = 0; Z < mapHeight + 1; Z++)
            {
                
                float amp = 1;
                float freq = 1;

                float noiseHeight = 0;

                for (int oct = 0; oct < this.octaves; oct++)
                {

                    float PerlinX = (float)X / (float)mapWidth * freq;
                    float PerlinZ = (float)Z / (float)mapHeight * freq;

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



    // debug tools
    
    private void OnDrawGizmos()
    {
        
        if(this.debugTools)
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

            // add triangles to show based on hit triangle
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
            

        }
    }

}
