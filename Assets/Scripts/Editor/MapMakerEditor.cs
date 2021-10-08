using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapMaker))]
public class MapMakerEditor : Editor
{
    MapMaker mapMaker;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        mapMaker = (MapMaker)target;

        mapMaker.BubbleNames = new string[mapMaker.Bubbles.Length];

        for (int i = 0; i < mapMaker.Bubbles.Length; i++)
        {
            mapMaker.BubbleNames[i] = mapMaker.Bubbles[i].name;
        }

        GUIContent selectedBubbleLabel = new GUIContent("Selected Bubble");
        mapMaker.SelectedBubbleNum = EditorGUILayout.Popup(selectedBubbleLabel, mapMaker.SelectedBubbleNum, mapMaker.BubbleNames);

        if (GUILayout.Button("Save Map"))
        {
            mapMaker.SaveMap();
        }

    }

    public void Awake()
    {

    }
}
