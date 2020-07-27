using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Camera renderCamera;
    [SerializeField] float delay;
    [SerializeField] RenderTexture renderTexture;


    private void Awake()
    {
        renderCamera.targetTexture = renderTexture;
        RTImage(renderCamera);
        RTImage(renderCamera);
        StartCoroutine(Render());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Texture2D RTImage(Camera camera)
    {
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        camera.Render();

        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        RenderTexture.active = currentRT;
        return image;
    }

    IEnumerator Render()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            RTImage(renderCamera);
        } 
    }
}
