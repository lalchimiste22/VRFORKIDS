using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleFlightCharacterController : RunnerCharacterController {

    public enum InputMode
    {
        Tilt,
        Aim
    }

    //The input as a (horizontal, vertical) vector
    private Vector2 _directionalInput;

    //The distance from the camera to the input plane
    public float InputPlaneDistance = 1.0f;

    //Speed on which to change the elevation
    public float ElevationSpeed = 2.0f;

    //Forward speed on which to move
    public float ForwardSpeed = 0.0f;

    //How we will process gaze input
    public InputMode Mode;

    //Only for Tilt mode
    public float TiltSensitivity = 0.01f;
    
    //The maximum amount of rotation the pawn will perform when confronted with a maximum directional input
    public Vector2 PawnMaxRotationInDegrees = new Vector2(45.0f, 45.0f);

    //Offset of the pawn when possesed by this controller
    private Vector3 _pawnOffset;

    //Collider used to hit obstacles
    private Collider _cachedPawnCollider;

    //Init
    private void Start()
    {
        if (Pawn != null)
        {
            _pawnOffset = transform.InverseTransformPoint(Pawn.position);
            _cachedPawnCollider = Pawn.GetComponentInChildren<Collider>();
        }
        else
        {
            _cachedPawnCollider = GetComponentInChildren<Collider>();
        }
    }

    // Update is called once per frame
    void Update () {
        if (!HasControl)
            return;

        ProcessGazeInput();
        UpdateElevation();
        UpdatePawn();
	}
    
    void ProcessGazeInput()
    {
        switch(Mode)
        {
            case InputMode.Tilt:
                //We use the camera local transform, so we move depending of the position of the character forward
                Vector3 rotation = Camera.main.transform.localEulerAngles;
                rotation.x = rotation.x > 180.0f ? rotation.x - 360.0f : rotation.x;
                rotation.y = rotation.y > 180.0f ? rotation.y - 360.0f : rotation.y;
                rotation.z = rotation.z > 180.0f ? rotation.z - 360.0f : rotation.z;
                
                Vector3 radRotation = rotation * Mathf.Deg2Rad * TiltSensitivity;
                _directionalInput.x = Mathf.Clamp(-radRotation.z, -1.0f, 1.0f);
                _directionalInput.y = Mathf.Clamp(-radRotation.x, -1.0f, 1.0f);
                break;
            case InputMode.Aim:
                //Reset the directional input
                _directionalInput = new Vector2();

                Vector3 cameraForward = Camera.main.transform.forward;
                Vector3 referenceVector = transform.forward;

                //We will use the following formula  =>  a = b / Dot(unit(a),unit(b))
                float dotResult = Vector3.Dot(cameraForward, referenceVector);

                //We won't apply input if we're perpendicular/looking away the input plane
                if (dotResult <= 0)
                    return;

                //TODO: Maybe abort if the input is greater than a certain distance
                float gazeToPlaneDistance = InputPlaneDistance / dotResult;

                //Get the projection vector on the input plane
                _directionalInput = cameraForward * gazeToPlaneDistance;
                _directionalInput.x = Mathf.Clamp(_directionalInput.x, -1.0f, 1.0f);
                _directionalInput.y = Mathf.Clamp(_directionalInput.y, -1.0f, 1.0f);
                break;
        }
        
    }

    void UpdateElevation()
    {
        Vector3 newPosition = transform.position + new Vector3(_directionalInput.x * ElevationSpeed, _directionalInput.y * ElevationSpeed, ForwardSpeed) * Time.deltaTime;
        
        if(RunnerGameMode.Instance.IsPointInsidePlayArea(newPosition))
        {
            transform.position = newPosition;
        }
    }

    void UpdatePawn()
    {
        if (!Pawn)
            return;

        Pawn.position = transform.TransformPoint(_pawnOffset);

        //Update rotation to match the directional input
        Vector2 rotation = PawnMaxRotationInDegrees * _directionalInput;
        Pawn.localEulerAngles = new Vector3(-rotation.y, Pawn.localEulerAngles.y, -rotation.x);
    }

    public override Collider GetControlledPawnCollider()
    {
        return _cachedPawnCollider;
    }
}
