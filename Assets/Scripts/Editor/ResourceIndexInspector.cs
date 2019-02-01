using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourceIndex))]
public class ResourceIndexInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ResourceIndex Index = (ResourceIndex)target;
        if(GUILayout.Button("Bake ResourceIndex"))
        {
            Index.Bake();
        }
    }
}
