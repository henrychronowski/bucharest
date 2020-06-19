using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class MapDistributer : MonoBehaviour
{

    [SerializeField] private Sprite sourceImg = null;
    [SerializeField] private Vector2 mapScale = new Vector2(1, 1);

    [SerializeField] private Transform viewer = null;
    [SerializeField] private static Vector2 viewerPosition;

    [SerializeField] private int viewerDistance = 5;


    [SerializeField] private bool autoUpdate = false;
    // will automatily run code 

    private bool debugTools = false;
    // debugging tools for devs

    private const int CHUNK_SIZE = 240;

    private List<MapChunk> loadedChuncks = new List<MapChunk>();



    //sets and gets
    public bool GetAutoUpdate()
    {
        return this.autoUpdate;
    }

    public void CreateMap()
    {
        // get map size
        int mapHeight = Mathf.CeilToInt(sourceImg.texture.height * mapScale[0]);
        int mapWidth = Mathf.CeilToInt(sourceImg.texture.width * mapScale[1]);

        // based on image dementions how many chucks will we need
        int chuncksTall = mapHeight / CHUNK_SIZE;
        int chuncksLong = mapWidth / CHUNK_SIZE;


        if (mapHeight % CHUNK_SIZE > 0)
        {
            chuncksTall += 1;
        }
        if (mapWidth % CHUNK_SIZE > 0)
        {
            chuncksLong += 1;
        }


        // load chunks
        MapChunk mapChunckPrefab = Resources.Load<MapChunk>("MapChunk") as MapChunk;
        Dictionary<int, BiomeData> biomeLogic = new Dictionary<int, BiomeData>();
        List<Color> BiomesFound = new List<Color>();

        BiomesFound.Add(new Color(0, 0, 0));
        biomeLogic.Add(0, new BiomeData(4, 1, 3, 3, AnimationCurve.Linear(1, 1, 1, 1)));

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
                            if (RGBequal(pixelColor, BiomesFound[i]))
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


                        // get the index of the biome and store it at the location in the biome map keeping track of biomes
                        landMap[chunkX, chunkY] = BiomesFound.IndexOf(pixelColor);




                        



                        //check to see if the biome has gen data already, if not create it
                        if (!biomeLogic.ContainsKey(landMap[chunkX, chunkY]))
                        {
                            Debug.Log(octs + " " + percs + " " + effect + " " + heightScale);
                            biomeLogic.Add(landMap[chunkX, chunkY], new BiomeData(octs, percs, effect, heightScale, AnimationCurve.Linear(1, 1, 1, 1)));

                            octs *= 1;
                            percs *= 2f;
                            effect *= 1;
                            heightScale *= 2;

                        }
                    }

                }


                //can be broken out for better rendering
                // generate map
                MapChunk mapChunck = GameObject.Instantiate(mapChunckPrefab, new Vector3(x * CHUNK_SIZE, 0, y * CHUNK_SIZE), transform.rotation, this.transform);
                mapChunck.GetComponent<MapChunk>().Generate(landMap, new Vector2(x, y), biomeLogic, 1, 10, CHUNK_SIZE);

            }
        }


        Debug.Log(BiomesFound.Count);

    }


    bool RGBequal(Color c1, Color c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b;
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