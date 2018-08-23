using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RunnerObstacleMovement : MonoBehaviour {

    public float Velocity = 10;
    public Vector3 Direction;

    //Player
    public RunnerCharacterController CharacterController;

    public float DistanceForClearance = 1.0f;

    private Rigidbody _rigidbody;
    private bool _hasCollided;
    private bool _pendingClearance;

	// Use this for initialization
	void Start () {
        _rigidbody = GetComponent<Rigidbody>();

        if (!CharacterController)
            Debug.Log("EagleFlightController must be setup for the obstacles to work");
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position += Direction * Velocity * Time.deltaTime; //@TODO: for now
        _rigidbody.velocity = Direction * Velocity;

        //Check if we have passed the character recently
        if(_pendingClearance && !_hasCollided && DidPassPlayerPawn())
        {
            RunnerGameMode.Instance.IncrementScore();
            _pendingClearance = false;
        }
    }

    private bool DidPassPlayerPawn()
    {
        Vector3 pawnPosition = CharacterController.GetControlledPawnCollider().transform.position;
        Vector3 nextFramePosition = transform.position + Direction * Velocity * Time.deltaTime;

        float currentDistance = Vector3.Distance(pawnPosition, transform.position);
        float nextFrameDistance = Vector3.Distance(pawnPosition, nextFramePosition);

        //If we have a greater distance from the player and we're currently moving AWAY from it
        return currentDistance > DistanceForClearance && nextFrameDistance > currentDistance;        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(CharacterController && other == CharacterController.GetControlledPawnCollider())
        {
            _hasCollided = true;
            Debug.Log("BOOM");
            //MyManager.Instance.cameraManager.Shake(1.0f); => needs UShake
        }
    }

    public void Reset()
    {
        _hasCollided = false;
        _pendingClearance = true;
    }
}
