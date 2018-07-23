using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Trans
{
    public Trans(Transform T)
    {
        position = T.position;
        rotation = T.rotation;
        scale = T.localScale;
    }

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

public class EaseMoveTo : MonoBehaviour {

    private bool bEasingIn = false;
    private Transform _transform;
    private Trans targetTransform;
    private Vector3 velocity;
    private float velocityAngleX;
    private float velocityAngleY;
    private float velocityAngleZ;
    public float SmoothTime = 1.0f;

    // Use this for initialization
    void Start () {
        _transform = gameObject.transform;
        velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        if (!bEasingIn)
            return;

        Vector3 nextPosition = Vector3.SmoothDamp(_transform.position, targetTransform.position, ref velocity, SmoothTime);
        float nextAngleX = Mathf.SmoothDampAngle(_transform.eulerAngles.x, targetTransform.rotation.eulerAngles.x, ref velocityAngleX, SmoothTime);
        float nextAngleY = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetTransform.rotation.eulerAngles.y, ref velocityAngleY, SmoothTime);
        float nextAngleZ = Mathf.SmoothDampAngle(_transform.eulerAngles.z, targetTransform.rotation.eulerAngles.z, ref velocityAngleZ, SmoothTime);

        gameObject.transform.position = nextPosition;
        gameObject.transform.eulerAngles = new Vector3(nextAngleX, nextAngleY, nextAngleZ);

        if (Mathf.Abs((nextPosition - targetTransform.position).magnitude) < 0.01 && Mathf.Abs(nextAngleX - targetTransform.rotation.eulerAngles.x) < 0.01 && Mathf.Abs(nextAngleY - targetTransform.rotation.eulerAngles.y) < 0.01 && Mathf.Abs(nextAngleZ - targetTransform.rotation.eulerAngles.z) < 0.01)
            bEasingIn = false;
    }

    public void SmoothTransport(Trans Destination)
    {
        bEasingIn = true;
        targetTransform = Destination;
    }
}
