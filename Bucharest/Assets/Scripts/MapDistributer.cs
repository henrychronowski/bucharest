using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDistributer : MonoBehaviour
{

    [SerializeField] private Sprite sourceImg = null;
    [SerializeField] private Vector2 mapScale = new Vector2(1,1);

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
        

        //create map chunks
        for (int y = 0; y < chuncksTall; y++)
        {
            for (int x = 0; x < chuncksLong; x++)
            {
                // create land map
                bool[,] landMap = new bool[CHUNK_SIZE, CHUNK_SIZE];
                int checkY, checkX;


                for (int chunkX = 0; chunkX < landMap.GetLength(0); chunkX++)
                {
                    for (int chunkY = 0; chunkY < landMap.GetLength(1); chunkY++)
                    {
                        checkY = (CHUNK_SIZE * y) + chunkY;
                        checkX = (CHUNK_SIZE * x) + chunkX;
                        landMap[chunkX, chunkY] = this.sourceImg.texture.GetPixel(checkX, checkY).grayscale > 0 ? true : false;
                        
                    }
                }

                MapChunk mapChunck = GameObject.Instantiate(mapChunckPrefab, new Vector3(x * CHUNK_SIZE,0, y * CHUNK_SIZE), transform.rotation, this.transform);
                mapChunck.GetComponent<MapChunk>().Generate(landMap, new Vector2(x, y), 4, 1, 3, 3, AnimationCurve.Linear(1,1,1,1), 0, 10, CHUNK_SIZE);

            }
        }
        
        


        // what kind of biome will it be
        // create chuck at location
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
