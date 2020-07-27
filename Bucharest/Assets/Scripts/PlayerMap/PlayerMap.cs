using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMap : MonoBehaviour
{
    public float smoothDistort;

    [SerializeField] Material mapMaterial;

    [SerializeField] private float refreshPoint;

    private void Awake()
    {
        mapMaterial = GetComponent<MeshRenderer>().material;
        refreshPoint = 1.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        refreshPoint -= smoothDistort;

        if (refreshPoint <= 0.0f)
            refreshPoint = 1.0f;

        mapMaterial.SetFloat("_ScanPoint", refreshPoint);
    }
}
