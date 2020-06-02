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
            if (mapGen.GetAutoUpdate())
            {
                mapGen.Generate();
            }
        }



        if (GUILayout.Button("Generate"))
        {
            mapGen.Generate();
        }
    }
}
