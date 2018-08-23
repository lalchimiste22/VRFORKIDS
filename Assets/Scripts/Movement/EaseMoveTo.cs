using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public bool bEasingIn { get; private set; }
    private Transform _transform;
    private Trans targetTransform;
    private Vector3 velocity;
    private float velocityAngleX;
    private float velocityAngleY;
    private float velocityAngleZ;
    public float DefaultSmoothTime = 1.0f;
    private float SmoothTime = 1.0f;
    public float CurrentTime { get; private set; }
    private Action OnCompletion;

    // Use this for initialization
    void Start () {
        _transform = gameObject.transform;
        velocity = Vector3.zero;
        bEasingIn = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!bEasingIn)
            return;

        CurrentTime += Time.deltaTime / SmoothTime;

        Vector3 nextPosition = Vector3.SmoothDamp(_transform.position, targetTransform.position, ref velocity, SmoothTime, float.PositiveInfinity, Time.fixedDeltaTime);

        float nextAngleX = Mathf.SmoothDampAngle(_transform.eulerAngles.x, targetTransform.rotation.eulerAngles.x, ref velocityAngleX, SmoothTime);
        float nextAngleY = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetTransform.rotation.eulerAngles.y, ref velocityAngleY, SmoothTime);
        float nextAngleZ = Mathf.SmoothDampAngle(_transform.eulerAngles.z, targetTransform.rotation.eulerAngles.z, ref velocityAngleZ, SmoothTime);

        gameObject.transform.position = nextPosition;
        gameObject.transform.eulerAngles = new Vector3(nextAngleX, nextAngleY, nextAngleZ);

        if (Mathf.Abs((nextPosition - targetTransform.position).magnitude) < 0.1 && Mathf.Abs(nextAngleX - targetTransform.rotation.eulerAngles.x) < 0.1 && Mathf.Abs(nextAngleY - targetTransform.rotation.eulerAngles.y) < 0.1 && Mathf.Abs(nextAngleZ - targetTransform.rotation.eulerAngles.z) < 0.1)
        {
            bEasingIn = false;

            if (OnCompletion != null)
                OnCompletion.Invoke();
        }
    }

    public void SmoothTransport(Trans Destination, float NewSmoothTime = -1.0f, Action InOnCompletion = null)
    {
        CurrentTime = 0.0f;
        bEasingIn = true;
        targetTransform = Destination;
        OnCompletion = InOnCompletion;
        velocity = Vector3.zero;

        SmoothTime = NewSmoothTime == -1.0f ? DefaultSmoothTime : NewSmoothTime;

    }

    public void Abort()
    {
        bEasingIn = false;
    }
}
