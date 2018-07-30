using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Linq;

[CustomEditor(typeof(WaypointMovement))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WaypointMovement myScript = (WaypointMovement)target;
        if (GUILayout.Button("Obtain waypoints from children's transform"))
        {
            myScript.ProcessChildTransforms();
        }
    }
}


public class WaypointMovement : MonoBehaviour {

    public UnityEvent OnPathCompleted;

    public Transform[] Waypoints;
    public float Velocity = 10.0f;
    public bool bEaseMovement = true;

    private Transform CurrentTransform;
    private int CurrentWaypointIndex;
    private Vector3 CurrentVelocity;
    private Vector3 AngleVelocity;

    //Create button for getting all the child transforms
    public void ProcessChildTransforms()
    {
        Waypoints = GetComponentsInChildren<Transform>().Skip(1).ToArray();
    }

    public void BeginWaypointMovement(Transform Target)
    {
        if (Waypoints.Length == 0)
            return;

        CurrentWaypointIndex = 0;
        CurrentTransform = Target;
        CurrentVelocity = bEaseMovement ? Vector3.zero : Vector3.one * Velocity;
    }
    
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (!CurrentTransform)
            return;

        //Get the direction
        Vector3 delta = (Waypoints[CurrentWaypointIndex].position - CurrentTransform.position);
        float distance = delta.sqrMagnitude;
        Vector3 direction = delta.normalized;

        Vector3 nextPosition = distance <= (Velocity * Velocity * Time.deltaTime) ? Waypoints[CurrentWaypointIndex].position  : CurrentTransform.position + direction * Velocity * Time.deltaTime;
        float nextAngleX = Mathf.SmoothDampAngle(CurrentTransform.eulerAngles.x, Waypoints[CurrentWaypointIndex].rotation.eulerAngles.x, ref AngleVelocity.x, 1.0f);
        float nextAngleY = Mathf.SmoothDampAngle(CurrentTransform.eulerAngles.y, Waypoints[CurrentWaypointIndex].rotation.eulerAngles.y, ref AngleVelocity.y, 1.0f);
        float nextAngleZ = Mathf.SmoothDampAngle(CurrentTransform.eulerAngles.z, Waypoints[CurrentWaypointIndex].rotation.eulerAngles.z, ref AngleVelocity.x, 1.0f);

        CurrentTransform.position = nextPosition;
        CurrentTransform.eulerAngles = AngleVelocity;

        if (distance < 0.01)
        {
            CurrentWaypointIndex++;

            //Check if finished
            if(CurrentWaypointIndex >= Waypoints.Length)
            {
                if (OnPathCompleted != null)
                    OnPathCompleted.Invoke();

                //Clear
                CurrentTransform = null;
            }
        }
    }
}
