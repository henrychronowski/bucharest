using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    public float persitance;
    public float lacunarity;

    public float meshHeightMultiplier;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;
    public void generateMap()
    {
        float[,] noiseMap = Noise.generateNoiseMap(mapWidth, mapHeight, noiseScale, seed, octaves, persitance, lacunarity, offset);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        //display.DrawNoiseMap(noiseMap);
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier), noiseMap);

    }
}
