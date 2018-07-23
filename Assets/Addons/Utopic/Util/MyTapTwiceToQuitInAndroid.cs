using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class MyTapTwiceToQuitInAndroid : MonoBehaviour {

	[HeaderAttribute("Have only 1 object with this component")]
	[Space(8)]
	[TooltipAttribute("Maximum seconds between the two taps")]
	public float tapTime = 1.5f;
	public GameObject activateObjectWhenTappingOnce;

	private bool isDoingCooldown = false;
	private Coroutine lastCallToCoroutine;

	void Awake () {
        if(activateObjectWhenTappingOnce)
		    activateObjectWhenTappingOnce.SetActive (false);
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (isDoingCooldown) {
				Application.Quit ();
			} else {
				activateObjectWhenTappingOnce.SetActive (true);
			}

			if (lastCallToCoroutine != null)
				StopCoroutine (lastCallToCoroutine);
			lastCallToCoroutine = StartCoroutine (startCooldown ());

		}
	}

	IEnumerator startCooldown() {
		isDoingCooldown = true;
		yield return new WaitForSeconds (tapTime);
		isDoingCooldown = false;
		activateObjectWhenTappingOnce.SetActive (false);
	}
}
