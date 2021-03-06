﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EaseMoveTo : MonoBehaviour {

    public bool bEasingIn { get; private set; }
    private Transform _transform;
    private Trans targetTransform;
    private Vector3 velocity;
    private float velocityAngleX;
    private float velocityAngleY;
    private float velocityAngleZ;
    private float SmoothTime = 1.0f;
    public float CurrentTime { get; private set; }
    private Action OnCompletion;
    private bool OnlyMovePosition;

    //Static helpers
    public static float DefaultSmoothTime { get { return 1.0f; } }

    // Use this for initialization
    void Awake () {
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
        gameObject.transform.position = nextPosition;

        float nextAngleX = 0.0f, nextAngleY = 0.0f, nextAngleZ = 0.0f;

        if(!OnlyMovePosition)
        {
            nextAngleX = Mathf.SmoothDampAngle(_transform.eulerAngles.x, targetTransform.rotation.eulerAngles.x, ref velocityAngleX, SmoothTime);
            nextAngleY = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetTransform.rotation.eulerAngles.y, ref velocityAngleY, SmoothTime);
            nextAngleZ = Mathf.SmoothDampAngle(_transform.eulerAngles.z, targetTransform.rotation.eulerAngles.z, ref velocityAngleZ, SmoothTime);
            gameObject.transform.eulerAngles = new Vector3(nextAngleX, nextAngleY, nextAngleZ);
        }

        if(OnlyMovePosition && Mathf.Abs((nextPosition - targetTransform.position).magnitude) < 0.1)
        {
            bEasingIn = false;

            if (OnCompletion != null)
                OnCompletion.Invoke();
        }
        else if (Mathf.Abs((nextPosition - targetTransform.position).magnitude) < 0.1 && Mathf.Abs(nextAngleX - targetTransform.rotation.eulerAngles.x) < 0.1 && Mathf.Abs(nextAngleY - targetTransform.rotation.eulerAngles.y) < 0.1 && Mathf.Abs(nextAngleZ - targetTransform.rotation.eulerAngles.z) < 0.1)
        {
            bEasingIn = false;

            if (OnCompletion != null)
                OnCompletion.Invoke();
        }
    }

    public void SmoothTransport(Trans Destination, float NewSmoothTime = -1.0f, Action InOnCompletion = null, bool InOnlyMovePosition = false)
    {
        CurrentTime = 0.0f;
        bEasingIn = true;
        targetTransform = Destination;
        OnCompletion = InOnCompletion;
        velocity = Vector3.zero;
        OnlyMovePosition = InOnlyMovePosition;

        SmoothTime = NewSmoothTime < 0 ? DefaultSmoothTime : NewSmoothTime;

    }

    public void Abort()
    {
        bEasingIn = false;
    }
}
