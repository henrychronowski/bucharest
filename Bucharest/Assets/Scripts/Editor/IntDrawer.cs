/*
 * Author: Henry Chronowski
 * Updated: 02/06/2020
 * Purpose: A custom int property drawer
 */

//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//[CustomPropertyDrawer(typeof(int))]
//public class IntDrawer : PropertyDrawer
//{
//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//	{
//		EditorGUI.BeginProperty(position, label, property);
//		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//		GUI.backgroundColor = Color.green;
//		EditorGUI.PropertyField(position, property, GUIContent.none);

//		EditorGUI.EndProperty();
//	}
//}
