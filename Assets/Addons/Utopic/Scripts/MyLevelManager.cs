using UnityEngine;
using UnityEngine.SceneManagement;

public class MyLevelManager : MonoBehaviour {

	public bool fadeIn = true;
	public bool fadeOut = true;


	void Awake () {
		SceneManager.sceneLoaded += OnLevelLoaded;
	}

	// Runs each time a scene is loaded, even when script is using DontDestroyOnLoad().
	void OnLevelLoaded(Scene scene, LoadSceneMode mode) {
		if (fadeIn)
			MyManager.Instance.cameraManager.FadeIn ();
	}



	public void LoadScene (string name) {
		StartCoroutine (_LoadScene (name));
	}
	System.Collections.IEnumerator _LoadScene (string name) {
		if (fadeOut) {
			MyManager.Instance.cameraManager.FadeOut ();
			yield return new WaitForSeconds (MyManager.Instance.cameraManager.fadeOutTime);
		}
		SceneManager.LoadScene (name);
	}


	public void ReplayLevel() {
		this.LoadScene (GetCurrentSceneName());
	}


	public bool IsThisTheLastLevel () {
		return SceneManager.sceneCountInBuildSettings - 1 == GetCurrentSceneIndex ();
	}

	private int GetCurrentSceneIndex()
	{
		return SceneManager.GetActiveScene ().buildIndex;
	}
	private string GetCurrentSceneName()
	{
		return SceneManager.GetActiveScene ().name;
	}

}
