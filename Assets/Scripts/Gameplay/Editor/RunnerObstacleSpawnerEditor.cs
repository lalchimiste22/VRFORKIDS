using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RunnerObstacleSpawner))]
public class RunnerObstacleSpawnerEditor : Editor {

    protected virtual void OnSceneGUI()
    {
        RunnerObstacleSpawner spawner = (RunnerObstacleSpawner)target;
        
        foreach(ObstacleSpawnInfo info in spawner.SpawnInfo)
        {
            if (!info.IsEditing)
                continue;

            for (int i = 0; i < info.SpawnAreaPoints.Length; i++)
            {
                Vector3 point = info.SpawnAreaPoints[i];
                Vector3 worldPoint = spawner.transform.TransformPoint(point);

                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition = Handles.PositionHandle(worldPoint, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spawner,"Change Target Area Vertex");
                    info.SpawnAreaPoints[i] = spawner.transform.InverseTransformPoint(newTargetPosition);
                }
            }
        }
    }

}
