using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ApplicationMode
{
    Normal,
    Preview
};

public class MyManager : MonoBehaviour {
    
	/* Singleton getters */
	public static MyManager Instance {get; private set;}

    /* ID of player associated with this session */
    public int PlayerId = 1;

    /* Server URL */
    public string baseUrl = "http://localhost:8000/";

    /* Fade image to use when requesting a cross fade */
    public UnityEngine.UI.Image FadeImage;

    /* Duration of the default cross fade */
    public float CrossFadeDuration = 1.5f;

    /* Custom time dilation to apply to the game */
	[RangeAttribute(0f,10f)]
	public float timeScale = 1f;
	public bool findTagPlayer = true;

    //Cached objects
	public GameObject playerGameObject { get; set;}
	public MyCameraManager cameraManager { get; private set;}
	public MyLevelManager loader { get; private set;}
	public MyAudioManager audioManager { get; private set;}

    /* The current application mode */
    public ApplicationMode Mode { get; private set; }

    private void Start()
    {
        //When starting, we want to make sure we aren't on a fade image
        if (FadeImage)
        {
            FadeImage.canvasRenderer.SetAlpha(0.0f);
        }
    }
    
    void Awake () {

        //As this class is a singleton, we only allow one instance to be alive.
		if (Instance == null)
        {
            //Setup singleton values
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //This will initialize important values, like the application mode, etc
            Mode = ApplicationMode.Normal; //We initialize the value first, we could be on a build that doesn't support web argument passing
            InitWebArguments();

            //Will need to manage level loading for resources
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelLoaded;

            //Cache useful components
            loader = GetComponent<MyLevelManager>();
            cameraManager = GetComponent<MyCameraManager>();
            audioManager = GetComponent<MyAudioManager>();

            //Make sure we have everything we need for normal behavior
            if (!(loader && cameraManager && audioManager))
            {
                Debug.LogError("Manager: Couldn't find all required components");
            }
        }
		else if (Instance != this)
        {
            //We already have a manager, eliminate this one and maintain the original.
            Destroy(gameObject);
        }
    }

    void InitWebArguments()
    {
        //Obtain the sections that conform this url (separated by the GET char)
        string[] UrlSections = Application.absoluteURL.Split('?');
        if(UrlSections.Length > 1)
        {
            //Arguments are on the second section of this URL
            string[] Arguments = UrlSections[1].Split('&');
            foreach(string Arg in Arguments)
            {
                //We need a final pass on the arguments, 
                string[] KeyValue = Arg.Split('=');
                ParseWebArgument(KeyValue[0], KeyValue[1]);
            }
        }
    }

    /// <summary>
    /// Manually selects each significant value and stores it on this object.
    /// </summary>
    /// <param name="Key">Name of the associated argument</param>
    /// <param name="Value">Value of the argument, which needs to be parsed down to the required type</param>
    void ParseWebArgument(string Key, string Value)
    {
        if(Key.ToLower() == "mode")
        {
            switch(Value.ToLower())
            {
                case "normal":
                    Mode = ApplicationMode.Normal;
                    break;
                case "preview":
                    Mode = ApplicationMode.Preview;
                    break;
                default:
                    Mode = ApplicationMode.Normal;
                    break;
            }

            Debug.Log("Entered application mode: " + Mode);
        }
    }

    // Runs each time a scene is loaded, even when script is using DontDestroyOnLoad().
    // Finds a GameObject tagged with 'Player'. Needs to find exactly one.
    void OnLevelLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) {
		if (findTagPlayer)
        {
			playerGameObject = null;
			GameObject[] goArr = GameObject.FindGameObjectsWithTag ("Player");
			if (goArr.Length > 0)
            {
				playerGameObject = goArr [0];
			}
		}

        //@Note: Support for custom time dilation
		Time.timeScale = this.timeScale;

        //Needed, as we could override the Manager and on level loaded this could be pending destruction
        if(this)
        {
            StartCoroutine(LoadResources());
        }
	}

	void OnValidate()
    {
        //@Note: Support for custom time dilation
        Time.timeScale = this.timeScale;
	}

    /// <summary>
    /// Obtains the resource codes currently available on the scene
    /// </summary>
    /// <param name="MappedResources">A search map for each resource, indexed with its code. </param>
    /// <returns>An array of the resource codes currently available. </returns>
    string[] GetResourceCodes(out Dictionary<string, Recurso> MappedResources)
    {
        //Initialize the out parameter
        MappedResources = new Dictionary<string, Recurso>();

        //Get all the resources
        Recurso[] resources = GameObject.FindObjectsOfType<Recurso>();

        //Map each code to the resource
        List<string> codes = new List<string>();
        foreach (Recurso r in resources)
        {
            if (!MappedResources.ContainsKey(r.Codigo))
            {
                codes.Add(r.Codigo);
                MappedResources.Add(r.Codigo, r);
            }
        }

        return codes.ToArray();
    }

    /// <summary>
    /// Obtains the resource codes currently available on the scene
    /// </summary>
    /// <returns>An array of the resource codes currently available. </returns>
    string[] GetResourceCodes()
    {
        Dictionary<string, Recurso> MappedResources;
        return GetResourceCodes(out MappedResources);
    }

    /// <summary>
    /// Loads every resource available on the level and updates their runtime info with the remote DB data.
    /// </summary>
    /// <returns>Enumerator for CoRoutine</returns>
    IEnumerator LoadResources()
    {
        Debug.Log("Loading resources...");

        //Obtain all the available codes
        Dictionary<string, Recurso> MappedResources;
        string[] codes = GetResourceCodes(out MappedResources);

        //Do a WWW call to the web server
        string url = baseUrl + "jugadores/" + PlayerId + "/recursos/" + string.Join(",", codes);
        Debug.Log("Calling URL:" + url);
        using (WWW www = new WWW(url))
        {
            yield return www;

            //Obtain the json data dictionary
            JSONObject JsonData = new JSONObject(www.text);
            
            //Data is separated as a dictionary
            if(JsonData.type == JSONObject.Type.OBJECT)
            {
                for (int i = 0; i < JsonData.list.Count; i++)
                {
                    string key = (string)JsonData.keys[i];
                    JSONObject j = (JSONObject)JsonData.list[i];

                    Recurso r = null;
                    MappedResources.TryGetValue(key, out r);

                    if (r)
                    {
                        r.SetupFromJsonData(j);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Performs a cross fade on the screen, calling the action after completion.
    /// </summary>
    /// <param name="OnCompletion">Handle of action to execute when done. </param>
    public void FadeOut(System.Action OnCompletion = null)
    {
        FadeImage.CrossFadeAlpha(1.0f, CrossFadeDuration, false);

        if(OnCompletion != null)
        {
            StartCoroutine(WaitForCrossFade(OnCompletion));
        }
    }

    /// <summary>
    /// Performs a cross fade on the screen, calling the action after completion.
    /// </summary>
    /// <param name="OnCompletion">Handle of action to execute when done.</param>
    public void FadeIn(System.Action OnCompletion = null)
    {
        FadeImage.CrossFadeAlpha(0.0f, CrossFadeDuration, false);

        if (OnCompletion != null)
        {
            StartCoroutine(WaitForCrossFade(OnCompletion));
        }
    }

    /// <summary>
    /// Waits for the cross fade operation to complete
    /// </summary>
    /// <param name="OnCompletion"> Action handle to run after completion. </param>
    /// <returns>Enumerator for Coroutine. </returns>
    IEnumerator WaitForCrossFade(System.Action OnCompletion)
    {
        yield return new WaitForSeconds(CrossFadeDuration);

        OnCompletion.Invoke();
    }
}
