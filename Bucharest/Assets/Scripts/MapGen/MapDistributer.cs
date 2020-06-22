using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;

using System.Threading;



public class MapDistributer : MonoBehaviour
{

    

    [SerializeField] private Sprite sourceImg = null;

    [SerializeField] private float biomeThreshHold = 0.5f;

    //[SerializeField] private Transform viewer = null;

    //[SerializeField] private static Vector2 viewerPosition;

    //[SerializeField] private int viewerDistance = 5;

    [SerializeField] private bool autoUpdate = false;
    // will automatily run code 

    [SerializeField] private bool debugTools = false;
    // debugging tools for devs

    [SerializeField] private List<BiomeData> biomeDatas = new List<BiomeData>();






    private Dictionary<int, BiomeData> biomeLogic = new Dictionary<int, BiomeData>();

    private List<Color> BiomesFound = new List<Color>();

    private Dictionary<Vector2, int[,]> landMaps = new Dictionary<Vector2, int[,]>();




    


    private const int CHUNK_SIZE = 240;

    private int chuncksTall, chuncksLong;



    //sets and gets
    public bool GetAutoUpdate()
    {
        return this.autoUpdate;
    }

    public void CreateMapData()
    {
        biomeDatas.Clear();
        BiomesFound.Clear();
        biomeLogic.Clear();
        landMaps.Clear();

        // get map size
        int mapHeight = Mathf.CeilToInt(sourceImg.texture.height);
        int mapWidth = Mathf.CeilToInt(sourceImg.texture.width);

        // based on image dementions how many chucks will we need
        chuncksTall = mapHeight / CHUNK_SIZE;
        chuncksLong = mapWidth / CHUNK_SIZE;


        if (mapHeight % CHUNK_SIZE > 0)
        {
            chuncksTall += 1;
        }
        if (mapWidth % CHUNK_SIZE > 0)
        {
            chuncksLong += 1;
        }


        // load chunks


        int octs = 4;
        float percs = 1f;
        int effect = 4;
        int heightScale = 2;


        for (int y = 0; y < chuncksTall; y++)
        {
            for (int x = 0; x < chuncksLong; x++)
            {
                // for every chunk
                int[,] landMap = new int[CHUNK_SIZE + 2, CHUNK_SIZE + 2];


                for (int chunkX = 0; chunkX < CHUNK_SIZE + 2; chunkX++)
                {
                    for (int chunkY = 0; chunkY < CHUNK_SIZE + 2; chunkY++)
                    {
                        // for every pixel in that chunk and bordering pixels

                        int imageX = x * CHUNK_SIZE + chunkX - 1;
                        int imageY = y * CHUNK_SIZE + chunkY - 1;


                        //if the pixel to check is within the bounds of the image
                        if ((imageX >= this.sourceImg.texture.width || imageY >= this.sourceImg.texture.height || imageY < 0 || imageX < 0))
                        {
                            if (imageY < 0)
                            {
                                imageY = 0;
                            }

                            if (imageX < 0)
                            {
                                imageX = 0;
                            }

                            if (imageX >= this.sourceImg.texture.width)
                            {
                                imageX = this.sourceImg.texture.width - 1;
                            }

                            if (imageY >= this.sourceImg.texture.height)
                            {
                                imageY = this.sourceImg.texture.height - 1;
                            }


                        }



                        // get the pixel color
                        Color pixelColor = this.sourceImg.texture.GetPixel(imageX, imageY);


                        // determin if it is a new biome
                        bool newBiome = true;
                        for(int i = 0; i < BiomesFound.Count; i++)
                        {
                            if (!BiomeCopare(pixelColor, BiomesFound[i], this.biomeThreshHold))
                            {
                                newBiome = false;
                            }
                        }


                        //if it is a new biome, Log it 
                        if (newBiome)
                        {
                            Debug.Log("<color=#"+ ColorUtility.ToHtmlStringRGB(pixelColor) + ">" + pixelColor.r + " " + pixelColor.g + " " + pixelColor.b + "</color>");
                            BiomesFound.Add(pixelColor);
                        }

                        // this and the previous loops may be better optimzed somehow but it works
                        
                        // get the index of the biome and store it at the location in the biome map keeping track of biomes
                        for (int i = 0; i < BiomesFound.Count; i++)
                        {
                            if (!BiomeCopare(pixelColor, BiomesFound[i], this.biomeThreshHold))
                            {
                                landMap[chunkX, chunkY] = i;
                            }
                        }


                        // create biome data
                        if (newBiome)
                        {

                            BiomeData biome = new BiomeData(octs, percs, effect, heightScale, AnimationCurve.Linear(1, 1, 1, 1));
                            
                            //for unity editor
                            biomeDatas.Add(biome);

                            //for map gen
                            biomeLogic.Add(landMap[chunkX, chunkY], biome);

                        }
                    }

                }
                // save land map
                landMaps.Add(new Vector2(x, y), landMap);
                
            }
        }
        Debug.Log(BiomesFound.Count);
        Debug.Log(biomeLogic.Count);
    }


    bool BiomeCopare(Color c1, Color c2, float threshHold)
    {
        // returns true if colors differtiate
        return (
                (Mathf.Abs(c1.r - c2.r) > threshHold) ||
                (Mathf.Abs(c1.g - c2.g) > threshHold) || 
                (Mathf.Abs(c1.b - c2.b) > threshHold)
                );
    }



    public void GenerateMap()
    {
        MapChunk mapChunckPrefab = Resources.Load<MapChunk>("MapChunk") as MapChunk;

        

        for (int biomeIdentifier = 0; biomeIdentifier < biomeDatas.Count; biomeIdentifier++)
        {
            biomeLogic[biomeIdentifier] = biomeDatas.ElementAt(biomeIdentifier);
        }


        for (int y = 0; y < chuncksTall; y++)
        {
            for (int x = 0; x < chuncksLong; x++)
            {
                MapChunk mapChunck = GameObject.Instantiate(mapChunckPrefab, new Vector3(x * CHUNK_SIZE, 0, y * CHUNK_SIZE), transform.rotation, this.transform);
                mapChunck.GetComponent<MapChunk>().Generate(landMaps[new Vector2(x, y)], new Vector2(x, y), biomeLogic, 6, 10, CHUNK_SIZE);
            }
        }

    }

    /*
    // debug tools
    private void OnDrawGizmos()
    {

        if (this.debugTools)
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
    */


}