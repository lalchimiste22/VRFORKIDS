using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TipoGameObjectValuePair : System.Object
{
    public Recurso.TipoContenido Key;
    public GameObject Value;
}

[RequireComponent(typeof(ZoomInDetail))]
public class Recurso : MonoBehaviour {
    public UnityEvent OnShownEvent;
    public UnityEvent OnHiddenEvent;

    public bool bOpenOnStart = false;
    private bool bShown = false;
    private bool bDeferedOpen = false;
    
    public enum TipoContenido
    {
        Texto,
        Pregunta,
        VoF,
        Pares
    }

    class Contenido
    {
        public static Contenido[] ParseFromJsonData(JSONObject Data)
        {
            List<Contenido> ListC = new List<Contenido>();
            foreach (JSONObject j in Data.list)
            {
                ListC.Add(new Contenido(j));
            }

            return ListC.ToArray();
        }

        public Contenido(JSONObject Data)
        {
            for (int i = 0; i < Data.list.Count; i++)
            {
                string key = (string)Data.keys[i];
                JSONObject j = (JSONObject)Data.list[i];

                //Setup the value depending of which 
                switch (key)
                {
                    case "data":
                        this.Data = System.Text.RegularExpressions.Regex.Unescape(j.str);
                        break;
                    case "imagen":
                        this.ImagenURL= j.str.Replace("\\","");
                        break;
                    case "tipo":
                        this.Tipo = stringToType(j.str);
                        break;
                    case "opciones":
                        this.Opciones = OpcionContenido.ParseFromJsonData(j);
                        break;
                    default:
                        break;
                }

            }
        }

        private TipoContenido stringToType(string type)
        {
            switch(type)
            {
                case "texto":
                    return TipoContenido.Texto;
                case "pregunta":
                    return TipoContenido.Pregunta;
                case "vof":
                    return TipoContenido.VoF;
                case "pares":
                    return TipoContenido.Pares;
                default:
                    return TipoContenido.Texto;
            }
        }


        public Texture2D TryGetImage(Recurso Instigator)
        {
            if (Imagen)
                return Imagen;

            //Image not loaded yet will query this object to receive the image when we load it
            BeginLoadImage(Instigator);

            return null;
        }

        private void BeginLoadImage(Recurso Instigator)
        {
            if (IsLoadingImage)
                return;
            
            CallbackLoader = Instigator;
            IsLoadingImage = true;
            Instigator.StartCoroutine(LoadImage());
        }

        private IEnumerator LoadImage()
        {
            Debug.Log("Starting Laod Image from url: " + ImagenURL);
            using (WWW www = new WWW(ImagenURL))
            {
                yield return www;

                if (www.error != null)
                {
                    Debug.LogError(www.error);
                    IsLoadingImage = false;
                }
                else
                {
                    Imagen = www.texture;
                    FinishLoadImage();
                }
            }
        }

        private void FinishLoadImage()
        {
            CallbackLoader.OnImageLoaded(Imagen, this);
            IsLoadingImage = false;
            Debug.Log("Image Loaded: " + Imagen);
        }


        //Tipo de contenido
        public TipoContenido Tipo;

        //Data general, puede sser 
        public string Data;

        //Url de imágen a desplegar
        public string ImagenURL;

        //Opciones de contenido si el tipo es pregunta
        public OpcionContenido[] Opciones;
        
        //Imágen a desplegar, tiene que ser cargada asyncronicamente
        private Texture2D Imagen;

        //Object that requested the load of the image, and wishes to be notified when it's completed
        private Recurso CallbackLoader;

        //Flag to know if we're already loading the image
        private bool IsLoadingImage;
    }

    public class OpcionContenido
    {
        public static OpcionContenido[] ParseFromJsonData(JSONObject Data)
        {
            List<OpcionContenido> ListC = new List<OpcionContenido>();
            foreach (JSONObject j in Data.list)
            {
                ListC.Add(new OpcionContenido(j));
            }

            return ListC.ToArray();
        }

        public OpcionContenido(JSONObject Data)
        {
            for (int i = 0; i < Data.list.Count; i++)
            {
                string key = (string)Data.keys[i];
                JSONObject j = (JSONObject)Data.list[i];

                //Setup the value depending of which 
                switch (key)
                {
                    case "data":
                        this.Data = j.str;
                        break;
                    case "data_secundaria":
                        this.DataSecundaria = j.str;
                        break;
                    case "tipo":
                        this.Tipo= j.str;
                        break;
                }
            }
        }

        //Data o texto de la opcion
        public string Data;

        //Si la opción es o no considerada correcta
        public string DataSecundaria;

        //Tipo
        public string Tipo;
    }

    //El código que representa a este recurso en el WS
    public string Codigo;

    public TipoGameObjectValuePair[] BlueprintPrefabs;

    private Dictionary<TipoContenido, GameObject> _blueprintPrefabs;

    //El nombre o título del recurso
    private string Nombre;

    //Descripcion del recurso, originalmente era el texto puesto
    private string Descripcion;

    //Contenidos
    private Contenido[] Contenidos;

    //Loading image to use when the web image hasn't been fully loaded
    public Texture2D LoadingImage;

    //The detail zoom to use
    private ZoomInDetail DetailZoom;

    // The page this control is at the moment
    private int CurrentPage;

    //The UI to show
    public GameObject UI;

    //UI Title
    private UnityEngine.UI.Text TitleText;

    //UI Detail
    private UnityEngine.UI.Text DetailText;

    //UI Image Wrapper
    private GameObject ImageWrapper;

    //UI Image
    private UnityEngine.UI.Image ContentImage;

    //UI Image Wrapper
    private GameObject OptionListWrapper;

    //The listview controller of the option list
    private ListViewController ListController;

    //Generates the data from the json string given
    public void SetupFromJsonData(JSONObject Data)
    {
        for (int i = 0; i < Data.list.Count; i++)
        {
            string key = (string)Data.keys[i];
            JSONObject j = (JSONObject)Data.list[i];

            //Setup the value depending of which 
            switch(key)
            {
                case "nombre":
                    this.Nombre = j.str;
                    break;
                case "descripcion":
                    this.Descripcion = j.str;
                    break;
                case "contenidos":
                    //This is an array
                    Contenidos = Contenido.ParseFromJsonData(j);
                    break;
                default:
                    break;
            }
            
        }
        
        //The title is common, so we should set it up here
        TitleText.text = Nombre;

        //Check if we're open, we should reset the current page
        if (bShown)
            GoToPage(CurrentPage);
    }

    void Awake()
    {
        DetailZoom = GetComponent<ZoomInDetail>();

        //UI Components, search for the text ones
        UnityEngine.UI.Text[] TextComponents = transform.GetComponentsInChildren<UnityEngine.UI.Text>(true);
        foreach(UnityEngine.UI.Text T in TextComponents)
        {
            if (T.gameObject.tag == "ResourceTitle")
                TitleText = T;
            else if (T.gameObject.tag == "ResourceDetail")
                DetailText = T;
        }

        //Image objects and wrappers
        UnityEngine.UI.Image[] ImageComponents = transform.GetComponentsInChildren<UnityEngine.UI.Image>(true);
        foreach (UnityEngine.UI.Image T in ImageComponents)
        {
            if (T.gameObject.tag == "ResourceImageWrapper")
            {
                ContentImage = T;
                ImageWrapper = ContentImage.transform.parent.gameObject;
                break;
            }
        }

        //Look for the optionlist
        ListController = transform.GetComponentInChildren<ListViewController>(true);
        OptionListWrapper = ListController.transform.parent.gameObject; //Looks awful, but the scroll has always this structure


        if (!TitleText || !DetailText || !ImageWrapper || !OptionListWrapper || !ListController)
            Debug.LogError("UI Components not found, maybe you changed their tag?");

        if (!DetailZoom)
            Debug.LogError("ZoomInDetail component not found, cannot show");

        if (!UI)
            Debug.LogError("No UI Component bound, cannot continue");

        //Generate the dictionary using the serializable keyvalue pair
        _blueprintPrefabs = new Dictionary<TipoContenido, GameObject>();
        foreach(TipoGameObjectValuePair KV in BlueprintPrefabs)
        {
            _blueprintPrefabs.Add(KV.Key, KV.Value);
        }

        //Manually hide, we don't want to start showing resources without them being selected
        UI.SetActive(false);

        //Check for auto open config
        bDeferedOpen = bOpenOnStart;
    }

    private void Update()
    {
        if(bDeferedOpen && MyManager.Instance.AppMode == ApplicationMode.Normal)
        {
            Show();
            bDeferedOpen = false;
        }
    }

    /// <summary>
    /// Shows the resource, displaying the first page if not specified and moving the player to the "Interest" point
    /// </summary>
    public void Show(int ShowPage = 0)
    {
        //Has to be first... if not, the internal IEnumerator from the yield will not run (as its deactivated)
        UI.SetActive(true);

        //Show the page if valid, if not, fallback to first page
        if (!GoToPage(ShowPage))
            GoToPage(0);

        //Show the detail zoom, which will trigger the movement and all
        DetailZoom.ShowZoomDetail();

        //Invoke the event
        OnShownEvent.Invoke();
        bShown = true;
    }

    /// <summary>
    /// Hides this control, returning control and position to the controller
    /// </summary>
    public void Hide()
    {
        //Unzoom the view
        DetailZoom.ExitZoomDetail();
        UI.SetActive(false);

        //Invoke the event
        OnHiddenEvent.Invoke();
        bShown = false;
    }

    /// <summary>
    /// Goes to the specified page and configures it acordingly
    /// </summary>
    /// <param name="Page">The index of the page to go to</param>
    /// <returns>If the page was indeed changed</returns>
    private bool GoToPage(int Page)
    {
        if (Contenidos == null || Page < 0 || Page >= Contenidos.Length)
            return false;

        //Change the content here
        Contenido currentContent = Contenidos[Page];
        DetailText.text = currentContent.Data;

        //Update the current index
        CurrentPage = Page;
        
        //Try to setup the image if there is one
        if(currentContent.ImagenURL.Length > 0)
        {
            //First we should enable the image wrapper
            ImageWrapper.SetActive(true);

            //There's an image url, should try to get it if it exists
            Texture2D image = currentContent.TryGetImage(this);
            if (image)
            {
                ContentImage.overrideSprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0, 0));
            }
            else
            {
                ContentImage.overrideSprite = Sprite.Create(LoadingImage, new Rect(0,0, LoadingImage.width, LoadingImage.height), new Vector2(0, 0));
            }

        }
        else
        {
            //No image wrapper needed
            ImageWrapper.SetActive(false);
        }

        //Check if we should update the list area
        if(UsesOptionListArea(currentContent))
        {
            OptionListWrapper.SetActive(true);
            ListController.Clear();

            FillOptionList(currentContent);
        }
        else
        {
            OptionListWrapper.SetActive(false);
        }

        return true;
    }

    /// <summary>
    /// Goes to the next page if valid, and configures it accordingly
    /// </summary>
    public void NextPage()
    {
        GoToPage(CurrentPage + 1);
    }

    /// <summary>
    /// Goes to the previous page if valid, and configures it accordingly
    /// </summary>
    public void PrevPage()
    {
        GoToPage(CurrentPage - 1);
    }

    private bool UsesOptionListArea(Contenido contenido)
    {
        return contenido.Tipo == TipoContenido.Pregunta || contenido.Tipo == TipoContenido.VoF || contenido.Tipo == TipoContenido.Pares;
    }

    private void FillOptionList(Contenido contenido)
    {
        IRenderOptionFactory factory = _blueprintPrefabs[contenido.Tipo].GetComponent<RenderOption>().GetFactory();
        RenderOption[] options = factory.BuildRenderOptions(contenido.Opciones);

        foreach(RenderOption Opt in options)
        {
            ListController.Add(Opt.gameObject);
        }

    }

    /// <summary>
    /// Called when we asked for an image when it was still loading and now has completed
    /// </summary>
    /// <param name="Image">The image loaded</param>
    /// <param name="From">The caller</param>
    private void OnImageLoaded(Texture2D Image, Contenido From)
    {
        //Check that we still want this image
        if (Contenidos[CurrentPage] != From)
            return;

        ContentImage.overrideSprite = Sprite.Create(Image, new Rect(0, 0, Image.width, Image.height), new Vector2(0, 0));
    }
}
