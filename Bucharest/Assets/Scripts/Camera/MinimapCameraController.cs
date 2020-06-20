using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Camera renderCamera;
    [SerializeField] float delay = 0.5f;
    [SerializeField] RenderTexture renderTexture;


    private void Awake()
    {
        renderCamera.targetTexture = renderTexture;

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

    IEnumerator Render()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log(renderCamera.activeTexture);
            renderCamera.activeTexture.Release();
            renderCamera.targetTexture = renderTexture;
        } 
    }
}
