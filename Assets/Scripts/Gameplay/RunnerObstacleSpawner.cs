using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class VarianceValue
{
    public float Value;
    public float Variance;
    
    public float Get()
    {
        return Random.Range(Value - Variance, Value + Variance);
    }
}

[System.Serializable]
public struct ObstacleSpawnInfo 
{
    //The blueprint to use
    public RunnerObstacleMovement BlueprintPrefab;

    //The vertices that define the spawn area
    public Vector3[] SpawnAreaPoints;

    [HideInInspector]
    public bool IsEditing;

    public Vector3 RandomPoint()
    {
        if (SpawnAreaPoints.Length == 0)
            return Vector3.zero;

        //@TODO: change this to actually use the AREA, not only the vertex
        return SpawnAreaPoints[Random.Range(0, SpawnAreaPoints.Length)];
    }
}

public class RunnerObstacleSpawner : MonoBehaviour {

    /// <summary>
    /// Reference to the running object pool
    /// </summary>
    public ObjectPool Pool;

    /// <summary>
    /// Prefab holding the 
    /// </summary>
    public GameObject FloorPrefab;

    /// <summary>
    /// Possible obstacles of the course
    /// </summary>
    public RunnerObstacleMovement[] Obstacles;

    /// <summary>
    /// Possible obstacles of the course
    /// </summary>
    public ObstacleSpawnInfo[] SpawnInfo;

    /// <summary>
    /// The character controller that will play this game
    /// </summary>
    public RunnerCharacterController CharacterController;

    /// <summary>
    /// Used for kill plane and recognizing obstacles as a whole
    /// </summary>
    [TagSelector]
    public string ObstacleTag;

    /// <summary>
    /// Period on which a variable will be spawned... with controlled random window
    /// </summary>
    public VarianceValue SpawnPeriod;

    /// <summary>
    /// If this spawner is currently activated
    /// </summary>
    public bool IsSpawning = true;

    /// <summary>
    /// Remaining time for a spawnable object to appear
    /// </summary>
    private float _spawnRemainingTime;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5F);

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        //Gizmos.matrix = rotationMatrix;

        Gizmos.DrawSphere(transform.position, 0.5f);

        //Start drawing the points
        foreach (ObstacleSpawnInfo info in SpawnInfo)
        {
            if (!info.IsEditing)
                continue;

            for (int i = 0; i < info.SpawnAreaPoints.Length; i++)
            {
                Vector3 point = info.SpawnAreaPoints[i];
                Vector3 worldPoint = transform.TransformPoint(point);

                //Chance to draw gizmos here
                Gizmos.color = Color.blue;

                //Draw this point first
                Gizmos.DrawSphere(worldPoint, 0.1f);

                //Draw the connecting line with the previous point
                if ((i - 1) >= 0)
                {
                    Gizmos.DrawLine(worldPoint, transform.TransformPoint(info.SpawnAreaPoints[i - 1]));
                }

                /*//Draw the fill area of the triangle and the previous 2 points, only when on the third point
                if(i % 3 == 2)
                {
                    
                }

                */

                if (i > 1 && i == info.SpawnAreaPoints.Length - 1) //Chance to fill the final segment
                {
                    Gizmos.DrawLine(worldPoint, transform.TransformPoint(info.SpawnAreaPoints[0]));
                }

            }
        }
    }
#endif


    // Use this for initialization
    void Start () {

        //Should start
        _spawnRemainingTime = SpawnPeriod.Get();

        if (!CharacterController)
            Debug.LogError("Must bind a character controller");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!IsSpawning)
            return;

        UpdateObstacleGeneration();
	}

    void UpdateObstacleGeneration()
    {
        if (Obstacles.Length == 0)
            return;

        if (_spawnRemainingTime <= 0)
        {
            //Reset time now, we can abort mid operation so its better to set it here
            _spawnRemainingTime = SpawnPeriod.Get();

            //Obtain an obstacle
            ObstacleSpawnInfo randomSpawnInfo = SpawnInfo[Random.Range(0, SpawnInfo.Length)];
            RunnerObstacleMovement obstacle = Pool.GetPooled<RunnerObstacleMovement>(randomSpawnInfo.BlueprintPrefab.gameObject);

            //Check for availability of pooled objects
            if (!obstacle)
                return;

            //Use the Obstacle info to get a randomized point inside the allowed spawn area
            obstacle.transform.position = transform.TransformPoint(randomSpawnInfo.RandomPoint());
            obstacle.Direction = this.transform.forward;
            obstacle.tag = ObstacleTag;
            obstacle.CharacterController = CharacterController;

            //Reset control flags
            obstacle.Reset();
        }

        _spawnRemainingTime -= Time.deltaTime;
    }
}
