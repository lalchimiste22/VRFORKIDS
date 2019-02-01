using UnityEngine;
using System.Collections;

public class LevelMusic : MonoBehaviour {

	public AudioClip levelMusic;
	[Range(0f,1f)] public float volume = 0.5f;
	public float delay = 0f;
	public float fadeIn = 1f;

	void Start () {
		Invoke ("_Start", delay);
	}
	void _Start () {
		MyManager.Instance.audioManager.PlayMusic (levelMusic, volume, fadeIn);
	}
}
