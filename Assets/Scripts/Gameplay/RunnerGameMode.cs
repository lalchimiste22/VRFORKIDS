using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RunnerGameMode : MonoBehaviour {

    public static readonly int PLAYAREA_LENGTH = 150;

    public float KillAreaDistance = -50.0f;

    //Singleton
    public static RunnerGameMode Instance { get; private set; }

    //Enable-Disable challenges
    public bool EnableChallenges = true;

    //How much score needed for a challenge
    public int ChallengeScoreRequirements = 3;

    //The resource to use as a base for challenging the player
    public Recurso ChallengeResource;

    //Runner controller
    public RunnerCharacterController CharacterController;

    //Obstacle Spawner
    public RunnerObstacleSpawner Spawner;

    //The cube dimensions of the play area, where the player will be able to move
    public Vector2 PlayArea = new Vector2(3.0f,3.0f);

    //Current score needed
    private int _scoreForNextChallenge = 3;
    private int _accumulatedScore = 0;
    private int _currentChallengePage = 0;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5F);

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Vector3 spawnerLocalPosition = transform.InverseTransformPoint(Spawner.transform.position);
        Vector3 size = new Vector3(PlayArea.x, PlayArea.y, (spawnerLocalPosition.z - KillAreaDistance));

        Vector3 origin = transform.TransformPoint(new Vector3(0.0f, 0.0f, spawnerLocalPosition.z + KillAreaDistance) / 2); //Middle point

        Gizmos.DrawCube(origin, size);
        Gizmos.DrawWireCube(origin, size);

        //Draw the kill area
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position + Vector3.forward * KillAreaDistance, new Vector3(PlayArea.x, PlayArea.y, 0.1f));

    }
#endif

    void Awake()
    {
        //Check if instance already exists
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (EnableChallenges && !ChallengeResource)
            Debug.LogError("Challenges are enabled, but no resource for challenge was bound");

        if (!CharacterController)
            Debug.LogError("No character controller bound.");
    }

    // Update is called once per frame
    void Update () {
	}

    public void IncrementScore(int increment = 1)
    {
        _accumulatedScore += increment;
        Debug.Log("Score UP! :" + _accumulatedScore);

        if (!EnableChallenges)
            return;

        _scoreForNextChallenge -= increment;

        if(_scoreForNextChallenge <= 0)
        {
            PresentChallenge();
            _scoreForNextChallenge = ChallengeScoreRequirements;
        }
    }

    public void PresentChallenge()
    {
        CharacterController.HasControl = false;
        Spawner.IsSpawning = false;

        ChallengeResource.Show(_currentChallengePage);
    }

    public bool IsPointInsidePlayArea(Vector3 Point)
    {
        Vector3 localPosition = transform.InverseTransformPoint(Point);

        //Important bits are the X,Y coordinates only
        Vector3 minPosition = transform.position - new Vector3(PlayArea.x, PlayArea.y) / 2;
        Vector3 maxPosition = transform.position + new Vector3(PlayArea.x, PlayArea.y) / 2;

        return localPosition.x > minPosition.x && localPosition.x < maxPosition.x && localPosition.y > minPosition.y && localPosition.y < maxPosition.y;
    }

    public bool DidPassKillPlane(Vector3 Position)
    {
        Vector3 localPosition = transform.InverseTransformPoint(Position);

        return KillAreaDistance > 0.0f ? localPosition.z > KillAreaDistance : localPosition.z < KillAreaDistance;
    }
}
