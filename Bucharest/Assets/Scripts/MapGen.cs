/* Author(s): Alexander Waters
 * Updated: mm/dd/yyyy
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
using UnityEngine;

public class MapGen : MonoBehaviour
{
    // properties
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private int seed;
    [SerializeField] private int heightScale;
    [SerializeField] private int octaves;
    [SerializeField] private float persitance;
    [SerializeField] private float effect;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private bool autoUpdate;
    [SerializeField] private bool debugTools = false;


    Vector3[] vertices;
    int[] triangles;

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
        // Create Vertices
        vertices = new Vector3[(this.mapWidth + 1) * (mapHeight + 1)];
        int i = 0;
        for (int X = 0; X < mapHeight + 1; X++)
        {
            for (int Z = 0; Z < this.mapWidth + 1; Z++)
            {
                float vertexHeight = NoiseMap[Z, X] * this.heightCurve.Evaluate(NoiseMap[Z, X]);
                vertices[i] = new Vector3(Z, vertexHeight, X);
                i++;
            }
        }

        // Create Triangles
        triangles = new int[(this.mapWidth) * (mapHeight) * 6];

        int verts = 0;
        int tris = 0;


        // add triangles by square
        for (int X = 0; X < this.mapWidth; X++)
        {
            for (int Z = 0; Z < mapHeight; Z++)
            {
                triangles[tris + 0] = verts + 0;
                triangles[tris + 1] = verts + mapHeight + 1;
                triangles[tris + 2] = verts + 1;

                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + mapHeight + 1;
                triangles[tris + 5] = verts + mapHeight + 2;

                verts++;
                tris += 6;
            }
            verts++;
        }


        // Create Mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        return mesh;
    }


    public void Generate()
    {
        float[,] NoiseMap = GenerateNoiseMaps(this.mapWidth, this.mapHeight);

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

                    float perlinValue = (Mathf.PerlinNoise(seed + PerlinX,seed + PerlinZ) * 2 - 1) * this.heightScale;

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
