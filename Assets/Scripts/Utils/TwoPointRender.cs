using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer)),ExecuteInEditMode]
public class TwoPointRender : MonoBehaviour {

    LineRenderer _lRenderer;

    /// <summary>
    /// The first end of the two point connection
    /// </summary>
    public Transform In;

    /// <summary>
    /// The second end of the two point connection
    /// </summary>
    public Transform Out;

	// Use this for initialization
	void Start () {
        _lRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!In || !Out)
            return;

        Vector3[] positions = new Vector3[2];
        positions[0] = In.position;
        positions[1] = Out.position;
        _lRenderer.SetPositions(positions);
	}
}
