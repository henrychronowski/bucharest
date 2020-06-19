using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeData { 

    public int octaves;

    public float persitance;

    public float effect;

    public int heightScale;

    public AnimationCurve heightCurve;

    public int levelOfDetail;

    public int seed;

    public BiomeData(int octaves, float persitance, float effect, int heightScale, AnimationCurve heightCurve)
    {
        this.octaves = octaves;

        this.persitance = persitance;

        this.effect = effect;

        this.heightScale = heightScale;

        this.heightCurve = heightCurve;

    }
}

