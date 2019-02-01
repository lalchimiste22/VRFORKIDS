using UnityEngine;
using System.Collections;

public class MoverCam : MonoBehaviour {

	public GameObject obj;
	public Vector3 finalPosition;
	public Vector3 finalRotation;

	// Use this for initialization
	void Start () {
		if (obj==null)
			obj = gameObject;
	}
	
	public void Mover() {
		obj.transform.Translate(finalPosition);
		obj.transform.Rotate(finalRotation);
	}
}
