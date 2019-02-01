using UnityEngine;
using System.Collections;

public class UCameraShake : MonoBehaviour {

	private float shakeAmount = 0.0f;
	private Vector3 targetLocalPosition;

	//How quickly is the shake decaying
	public float shakeDecay = 2.0f;

	void Start()
	{
		targetLocalPosition = transform.localPosition;
	}

	void Update()
	{
		if (Time.deltaTime > 0.0f)
		{
			// set our position to a random value (this depends on knowing that the unshaken local-position is at the origin)
			transform.localPosition = targetLocalPosition + Random.insideUnitSphere * shakeAmount;
			// fade the shake amount towards zero
			shakeAmount = Mathf.Lerp(shakeAmount, 0.0f, shakeDecay * Time.deltaTime);
		}
	}

	public void Shake(float amount)
	{
		shakeAmount = amount;
	}
}
