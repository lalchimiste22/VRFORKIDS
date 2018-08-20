using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RunnerObstacleMovement : MonoBehaviour {

    public float Velocity = 10;
    public Vector3 Direction;

    private Rigidbody _rigidbody;

	// Use this for initialization
	void Start () {
        _rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position += Direction * Velocity * Time.deltaTime; //@TODO: for now
        _rigidbody.velocity = Direction * Velocity;
    }
}
