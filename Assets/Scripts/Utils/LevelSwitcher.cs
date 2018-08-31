using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitcher : MonoBehaviour {

    public string DefaultPath = "Scenes/Interactive/MainMenu";

    private static string PATH_NONE = "";

	// Use this for initialization
	public void SwitchLevel(string path = "")
    {
        MyManager.Instance.loader.LoadScene(path.Equals(PATH_NONE) ? DefaultPath : path);
    }

}
