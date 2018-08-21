using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EagleFlightCharacterController : MonoBehaviour {

    public enum InputMode
    {
        Tilt,
        Aim
    }

    //The input as a (horizontal, vertical) vector
    private Vector2 _directionalInput;

    //The distance from the camera to the input plane
    public float InpudPlaneDistance = 1.0f;

    //Speed on which to change the elevation
    public float ElevationSpeed = 2.0f;

    //Forward speed on which to move
    public float ForwardSpeed = 0.0f;

    //How we will process gaze input
    public InputMode Mode;

    //Only for Tilt mode
    public float TiltSensitivity = 0.01f;

    //Rigidbody used for collision detection
    private Rigidbody _rigidbody;

    //Init
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {

        ProcessGazeInput();
        UpdateElevation();

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
                float gazeToPlaneDistance = InpudPlaneDistance / dotResult;

                //Get the projection veector on the input plane
                _directionalInput = cameraForward * gazeToPlaneDistance;
                _directionalInput.x = Mathf.Clamp(_directionalInput.x, -1.0f, 1.0f);
                _directionalInput.y = Mathf.Clamp(_directionalInput.y, -1.0f, 1.0f);
                break;
        }
        
    }

    void UpdateElevation()
    {
        //transform.position += new Vector3(_directionalInput.x * ElevationSpeed, _directionalInput.y * ElevationSpeed, ForwardSpeed) * Time.deltaTime;
        //_rigidbody.AddForce(new Vector3(_directionalInput.x * ElevationSpeed, _directionalInput.y * ElevationSpeed, ForwardSpeed) * Time.deltaTime);
        _rigidbody.velocity = new Vector3(_directionalInput.x * ElevationSpeed, _directionalInput.y * ElevationSpeed, ForwardSpeed);// * Time.deltaTime;
    }
}
