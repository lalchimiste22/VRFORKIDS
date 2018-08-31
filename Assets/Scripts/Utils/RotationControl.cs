using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationControl : MonoBehaviour {

    /// <summary>
    /// Target to apply the rotation to
    /// </summary>
    public Transform Target;

    /// <summary>
    /// Speed in which to rotate the target transform
    /// </summary>
    public float AnglePerSecond = 45;

    public void Start()
    {
        if (!Target)
            Debug.LogError("No target for rotation set");
    }

	// Use this for initialization
	public void RotateClockwise()
    {
        Vector3 rotationAngles = transform.TransformDirection(Vector3.up) * AnglePerSecond;
        Target.eulerAngles += rotationAngles * Time.deltaTime;
    }

    public void RotateCounterClockwise()
    {
        Vector3 rotationAngles = transform.TransformDirection(Vector3.down) * AnglePerSecond;
        Target.eulerAngles += rotationAngles * Time.deltaTime;
    }

    public void RotateTopDown()
    {
        Vector3 rotationAngles = transform.TransformDirection(Vector3.left) * AnglePerSecond;
        Target.eulerAngles += rotationAngles * Time.deltaTime;
    }

    public void RotateDownTop()
    {
        Vector3 rotationAngles = transform.TransformDirection(Vector3.right) * AnglePerSecond;
        Target.eulerAngles += rotationAngles * Time.deltaTime;
    }
}
