using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float [,] heightMap, float heightMultiplier)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;


        MeshData meshData = new MeshData(width, height);

        int verxtIndex = 0;

        for (int y= 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                meshData.vertices[verxtIndex] = new Vector3(x + topLeftX,heightMap[x,y] * heightMultiplier, topLeftZ - y);
                meshData.uvs[verxtIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width-1 && y < height-1)
                {
                    meshData.addTriangle(verxtIndex, verxtIndex + width + 1, verxtIndex + width);
                    meshData.addTriangle(verxtIndex + width + 1, verxtIndex , verxtIndex + 1);

                }
                verxtIndex++;


            }
        }

        return meshData;
    }
}


public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;

    public Vector2[] uvs;


    int triIndex;
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight-1) * 6];


    }


    public void addTriangle(int a, int b, int c)
    {
        triangles[triIndex] = a;
        triangles[triIndex+1] = b;
        triangles[triIndex+2] = c;
        triIndex += 3;
    }


    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}