/* Author(s): Alexander Waters
 * Updated: 6/3/2020
 * Resources: 
 * https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
 * Purpose: interacts Mapgen script with editor
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapDistributer))]
public class MapDistributerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapDistributer mapDistributer = (MapDistributer)target;
        if (DrawDefaultInspector())
        {
            if (mapDistributer.GetAutoUpdate())
            {
                mapDistributer.CreateMapData();
            }
        }



        if (GUILayout.Button("Create Map Data"))
        {
            mapDistributer.CreateMapData();
            
        }



        if (GUILayout.Button("Generate Map"))
        {
            mapDistributer.GenerateMap();

        }

        
    }
}