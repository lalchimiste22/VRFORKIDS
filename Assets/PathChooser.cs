using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathChooser : MonoBehaviour {

    public UnityEvent OnBeginDecision;
    public UnityEvent OnDecisionTaken;

    public WaypointMovement[] Paths;

    private EaseMoveTo CurrentMovement;

    public void BeginDecision(EaseMoveTo MovementComponent)
    {
        MovementComponent.SmoothTransport(new Trans(transform));
        CurrentMovement = MovementComponent;

        if (OnBeginDecision != null)
            OnBeginDecision.Invoke();
    }

    public void Decide(int PathIndex)
    {
        if (PathIndex >= Paths.Length)
            return;

        //Must clear any movement on the target
        CurrentMovement.Abort();
        Paths[PathIndex].BeginWaypointMovement(CurrentMovement.transform); //Uses a different type of movement

        if (OnDecisionTaken != null)
            OnDecisionTaken.Invoke();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
