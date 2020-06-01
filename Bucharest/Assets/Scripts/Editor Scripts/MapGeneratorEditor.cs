using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGen))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGen mapGen = (MapGen)target;
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.generate();
            }
        }



        if (GUILayout.Button("Generate"))
        {
            mapGen.generate();
        }
    }
}
