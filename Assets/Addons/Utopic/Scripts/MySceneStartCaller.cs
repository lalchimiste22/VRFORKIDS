using UnityEngine;
using System.Collections;

public class MySceneStartCaller : MonoBehaviour {

	public float timeForDelayedEvents;
	public UnityEngine.Events.UnityEvent onStart;
	public UnityEngine.Events.UnityEvent onStartDelayed;

	// Use this for initialization
	void Start () {
		onStart.Invoke ();
		Invoke ("invokeDelayedEvents", timeForDelayedEvents);
	}
	void invokeDelayedEvents() {
		onStartDelayed.Invoke ();
	}
}
