﻿/* Author(s): Alexander Waters
 * Updated: 6/3/2020
 * Resources: 
 * https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
 * Purpose: interacts Mapgen script with editor
 */

#if UNITY_EDITOR


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[UnityEditor.CustomEditor(typeof(MapGen))]
public class MapGeneratorEditor : UnityEditor.Editor
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

#endif