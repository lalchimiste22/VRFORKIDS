using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            RunnerObstacleMovement obstacle = Pool.GetPooled<RunnerObstacleMovement>(Obstacles[Random.Range(0, Obstacles.Length)].gameObject);

            //Check for availability of pooled objects
            if (!obstacle)
                return;

            //@TODO for now, change the transform to be of this same origin
            obstacle.transform.position = this.transform.position;
            obstacle.Direction = this.transform.forward;
            obstacle.tag = ObstacleTag;
            obstacle.CharacterController = CharacterController;

            //Reset control flags
            obstacle.Reset();
        }

        _spawnRemainingTime -= Time.deltaTime;
    }
}
