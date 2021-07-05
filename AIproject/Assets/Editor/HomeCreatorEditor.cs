using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HomeCreator))]
public class HomeCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HomeCreator homeCreator = (HomeCreator)target;
        if (GUILayout.Button("CreateNewHome"))
        {
            homeCreator.CreateHome();
        }
    }
}
