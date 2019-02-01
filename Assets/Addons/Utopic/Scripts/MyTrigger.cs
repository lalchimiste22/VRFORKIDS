using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class MyTrigger : MonoBehaviour {

	public bool enterOnce = true;
	public bool exitOnce = true;
	public UnityEngine.Events.UnityEvent onEnter;
	public UnityEngine.Events.UnityEvent onExit;
	public UnityEngine.Events.UnityEvent onStay;

	private Collider collider;
	private bool m_enterEnabled = true, m_exitEnabled = true;

	// Use this for initialization
	void Start () {
		collider = GetComponent<Collider> ();
	}

	void OnTriggerEnter() {
		if (m_enterEnabled) {
			onEnter.Invoke ();
		}
		if (enterOnce)
			m_enterEnabled = false;
	}
	void OnTriggerExit() {
		if (m_exitEnabled) {
			onExit.Invoke ();
		}
		if (exitOnce)
			m_exitEnabled = false;
	}
	void OnTriggerStay() {
		onStay.Invoke ();
	}
}