using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMap : MonoBehaviour
{
    public float smoothRefresh;
    public float smoothDistort;
    public float interval;

    [SerializeField] Material mapMaterial;

    [SerializeField] private float refreshPoint;
    private float distortion;
    private float sD;

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

    IEnumerator distort()
    {
        float current = 0.0f;
        float target = normalRandom(-interval, interval);
        distortion = current;
        while (true)
        {
            current = Mathf.MoveTowards(current, target, sD * Time.deltaTime);
            distortion = current;
            if(current == target)
            {
                yield return new WaitForSeconds(Random.Range(0.0f, 0.1f));
                target = normalRandom(-interval, interval);
            }
            yield return null;
        }
    }

    float normalRandom(float min, float max)
    {
        sD = smoothDistort * Random.Range(0.2f, 1.0f);
        return Random.Range(min, max);
    }
}
