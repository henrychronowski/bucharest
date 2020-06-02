using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(MapTemplate))]
public class MapTemplateEditor : Editor
{

    /*
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapTemplate mapTemp = (MapTemplate)target;

        Sprite mapTempImg = mapTemp.getSourceImg();

        Debug.Log(mapTempImg.texture.GetPixel(0,0).grayscale);
        
    }*/





}