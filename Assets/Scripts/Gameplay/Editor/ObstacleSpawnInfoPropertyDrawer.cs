using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ObstacleSpawnInfo))]
public class ObstacleSpawnInfoPropertyDrawer : PropertyDrawer{
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);
        
        if (!property.isExpanded)
            return;

        SerializedProperty isEditingProperty = property.FindPropertyRelative("IsEditing");
        bool isEditing = isEditingProperty.boolValue;
        string buttonLabel = isEditing ? "Stop Editing" : "Edit Area";

        if (GUI.Button(new Rect(position.xMin + 30f, position.yMax - 20f, position.width - 30f, 20f), buttonLabel))
        {
            isEditingProperty.boolValue = !isEditing;
        }

        
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
            return EditorGUI.GetPropertyHeight(property) + 20f;
        return EditorGUI.GetPropertyHeight(property);
    }

    void OnSceneGUI(SceneView sceneView)
    {
        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.PositionHandle(new Vector3(0f,0f,0f), Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
//             Undo.RecordObject(example, "Change Look At Target Position");
//             example.targetPosition = newTargetPosition;
//             example.Update();
        }
    }
}
