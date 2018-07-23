using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStereoClippingPlanes : MonoBehaviour {

    public float ClippingPlane = 200;

	// Use this for initialization
	void Start () {
        StartCoroutine(SetCamera());
	}
	
    IEnumerator SetCamera()
    {
        yield return new WaitForSeconds(1);
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            cam.farClipPlane = 200;
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
