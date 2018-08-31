using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyManager : MonoBehaviour {

	//this class is a singleton)
	public static MyManager Instance {get; private set;}

    //The player associated with this game
    public int PlayerId = 1;

    //Server URL
    public string baseUrl = "http://localhost:8000/";

    public UnityEngine.UI.Image FadeImage;

    public float CrossFadeDuration = 1.5f;

	[RangeAttribute(0f,10f)]
	public float timeScale = 1f;
	public bool findTagPlayer = true;

	public GameObject playerGameObject { get; set;}
	public MyCameraManager cameraManager { get; private set;}
	public MyLevelManager loader { get; private set;}
	public MyAudioManager audioManager { get; private set;}

    private void Start()
    {
        if (FadeImage)
            FadeImage.canvasRenderer.SetAlpha(0.0f);
    }

    void Awake () {
		// Singleton:
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy (this.gameObject); // Nota: Esto podria eliminar todo un objeto, con otros componentes
		DontDestroyOnLoad (gameObject);
		// End Singleton


		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelLoaded;


		loader = GetComponent<MyLevelManager> ();
		cameraManager = GetComponent<MyCameraManager> ();
		audioManager = GetComponent<MyAudioManager> ();

		if (!(loader && cameraManager && audioManager)) {
			//Debug.LogError ("MyError: MyManager coudn't find all the components and references.");
		}
	}


	// Runs each time a scene is loaded, even when script is using DontDestroyOnLoad().
	// Finds a GameObject tagged with 'Player'. Needs to find exactly one.
	void OnLevelLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) {
		if (findTagPlayer) {
			playerGameObject = null;
			GameObject[] goArr = GameObject.FindGameObjectsWithTag ("Player");
			if (goArr.Length > 0) {
				playerGameObject = goArr [0];
			}
		}

		Time.timeScale = this.timeScale;

        //Search for resources that exist on this map

        //Needed, as we could override the Manager and on level loaded this could be pending destruction
        if(this)
            StartCoroutine(LoadResources());
	}

	void OnValidate() {
		Time.timeScale = this.timeScale;
	}

    IEnumerator LoadResources()
    {
        Debug.Log("Collecting resources");

        //Get all the resources
        Recurso[] resources = GameObject.FindObjectsOfType<Recurso>();
        
        List<string> codes = new List<string>();
        Dictionary<string, Recurso> mappedResources = new Dictionary<string, Recurso>();
        foreach (Recurso r in resources)
        {
            if (mappedResources.ContainsKey(r.Codigo))
                continue;

            codes.Add(r.Codigo);
            mappedResources.Add(r.Codigo, r);
        }

        //Do a WWW call to the webserver
        string url = baseUrl + "jugadores/" + PlayerId + "/recursos/" + string.Join(",", codes.ToArray());
        Debug.Log("Calling URL:" + url);
        using (WWW www = new WWW(url))
        {
            yield return www;

            //Obtain the json data dictinoary
            JSONObject JsonData = new JSONObject(www.text);
            
            //Data is separated as a dictionary
            if(JsonData.type == JSONObject.Type.OBJECT)
            {
                for (int i = 0; i < JsonData.list.Count; i++)
                {
                    string key = (string)JsonData.keys[i];
                    JSONObject j = (JSONObject)JsonData.list[i];

                    Recurso r = null;
                    mappedResources.TryGetValue(key, out r);

                    if (r)
                        r.SetupFromJsonData(j);
                }
            }
        }
    }

    public void FadeOut(System.Action OnCompletion = null)
    {
        FadeImage.CrossFadeAlpha(1.0f, CrossFadeDuration, false);

        if(OnCompletion != null)
            StartCoroutine(WaitForCrossFade(OnCompletion));
    }

    public void FadeIn(System.Action OnCompletion = null)
    {
        FadeImage.CrossFadeAlpha(0.0f, CrossFadeDuration, false);

        if (OnCompletion != null)
            StartCoroutine(WaitForCrossFade(OnCompletion));
    }

    IEnumerator WaitForCrossFade(System.Action OnCompletion)
    {
        yield return new WaitForSeconds(CrossFadeDuration);

        OnCompletion.Invoke();
    }
}
