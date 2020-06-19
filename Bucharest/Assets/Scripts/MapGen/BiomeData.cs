using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct BiomeData {

    [SerializeField] public int octaves;

    [SerializeField] public float persitance;

    [SerializeField] public float effect;

    [SerializeField] public int heightScale;

    [SerializeField] public AnimationCurve heightCurve;


    public BiomeData(int octaves, float persitance, float effect, int heightScale, AnimationCurve heightCurve)
    {
        this.octaves = octaves;

        this.persitance = persitance;

        this.effect = effect;

        this.heightScale = heightScale;

        this.heightCurve = heightCurve;

    }
}

