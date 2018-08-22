using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerGameMode : MonoBehaviour {

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

    //Current score needed
    private int _scoreForNextChallenge = 3;
    private int _accumulatedScore = 0;
    private int _currentChallengePage = 0;

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
}
