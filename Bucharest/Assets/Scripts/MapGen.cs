using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TreeEditor;
using UnityEngine;

public class MapGen : MonoBehaviour
{


    public int mapWidth;
    public int mapHeight;
    public int heightScale;
    public int octaves;
    public float persitance;
    public float effect;
    public bool autoUpdate;


    // Start is called before the first frame update
    void Start()
    {
        float[,] NoiseMap = generateNoiseMaps(mapWidth, mapHeight);

        GetComponent<MeshFilter>().mesh = createMesh(NoiseMap);


    }

    public void generate()
    {
        float[,] NoiseMap = generateNoiseMaps(mapWidth, mapHeight);

        GetComponent<MeshFilter>().mesh = createMesh(NoiseMap);
    }


    float[,] generateNoiseMaps(int mapWidth, int mapHeight)
    {
        // create a item to return 
        float[,] NoiseMap = new float[mapWidth + 1, mapHeight + 1];

        // generate map heights
        for (int X = 0; X < mapWidth + 1; X++)
        {
            for (int Z = 0; Z < mapHeight + 1; Z++)
            {
                
                float amp = 1;
                float freq = 1;

                float noiseHeight = 0;

                for (int oct = 0; oct < octaves; oct++)
                {

                    float PerlinX = (float)X / (float)mapWidth * freq;
                    float PerlinZ = (float)Z / (float)mapHeight * freq;

                    float perlinValue = (Mathf.PerlinNoise(PerlinX, PerlinZ) * 2 - 1) * heightScale;

                    noiseHeight += perlinValue * amp;

                    amp *= persitance;
                    freq *= effect;

                    NoiseMap[X, Z] = noiseHeight;
                }



            }
        }


        return NoiseMap;
    }



    Mesh createMesh(float[,] NoiseMap)
    {

        // Create Vertices
        Vector3[] vertices = new Vector3[(mapWidth + 1) * (mapHeight + 1)];
        int i = 0;
        for (int X = 0; X < mapWidth + 1; X++)
        {
            for (int Z = 0; Z< mapHeight + 1; Z++)
            {
                vertices[i] = new Vector3(X, NoiseMap[X,Z], Z);
                i++;
            }
        }





        // Create Triangles
        int[] triangles = new int[(mapWidth) * (mapHeight) * 6];

        int verts = 0;
        int tris = 0;


        //add triangles by square
        for (int X = 0; X < mapWidth; X++)
        {
            for (int Z = 0; Z < mapHeight; Z++)
            {
                triangles[tris + 0] = verts + 1;
                triangles[tris + 1] = verts + mapWidth + 1;
                triangles[tris + 2] = verts + 0;

                triangles[tris + 3] = verts + mapWidth + 2;
                triangles[tris + 4] = verts + mapWidth + 1;
                triangles[tris + 5] = verts + 1;

                verts++;
                tris += 6;
            }
            verts++;
        }
        

        //Create Mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        return mesh;
    }




}
