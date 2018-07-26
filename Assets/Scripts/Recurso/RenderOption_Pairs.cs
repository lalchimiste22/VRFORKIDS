using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RenderOption_PairsFactory : IRenderOptionFactory
{
    private GameObject Blueprint;

    public RenderOption_PairsFactory(GameObject InBlueprint)
    {
        if (!InBlueprint.GetComponent<RenderOption_Pairs>())
        {
            Debug.LogError("Expected RenderOption_Pairs component on Blueprint GameObject, not found");
        }
        else
        {
            Blueprint = InBlueprint;
        }
    }

    public RenderOption[] BuildRenderOptions(Recurso.OpcionContenido[] Options)
    {
        List<RenderOption_Pairs> RenderOptions = new List<RenderOption_Pairs>();

        //First pass, create the RenderOptions and assign the first Port
        foreach (Recurso.OpcionContenido Opt in Options)
        {
            GameObject instantiated = GameObject.Instantiate(Blueprint);
            RenderOption_Pairs pair = instantiated.GetComponent<RenderOption_Pairs>();
            RenderOptions.Add(pair);

            pair.SetupPort(RenderOption_Pairs.PortLocation.Left, Opt);
        }

        //Second pass, rumble the options and assign the second port
        System.Random rnd = new System.Random();
        Recurso.OpcionContenido[] RandOpts = Options.OrderBy(x => rnd.Next()).ToArray();

        for (int i = 0; i < RandOpts.Length; i++)
        {
            Recurso.OpcionContenido opt = RandOpts[i];
            RenderOption_Pairs pair = RenderOptions[i];

            pair.SetupPort(RenderOption_Pairs.PortLocation.Right, opt);
        }

        return RenderOptions.ToArray();
    }
}


public class RenderOption_Pairs : RenderOption {

    enum PortType
    {
        Unknown,
        Text,
        Image
    }

    public enum PortLocation
    {
        Left,
        Right
    }
    
    //Left side
    public UnityEngine.UI.Text LabelLeft;
    public UnityEngine.UI.Image ImageLeft;

    public UnityEngine.UI.Text LabelRight;
    public UnityEngine.UI.Image ImageRight;

    public Socket LeftSocket;
    public Socket RightSocket;

    private PortType LeftPortType = PortType.Text;
    private PortType RightPortType = PortType.Text;

    public override IRenderOptionFactory GetFactory()
    {
        return new RenderOption_PairsFactory(this.gameObject);
    }
    
    public void SetupPort(PortLocation Location, Recurso.OpcionContenido Opcion)
    {
        PortType type = GetPortTypeForLocation(Location, Opcion);

        UnityEngine.UI.Text Label = null;
        string data = "";
        
        //Obtain the correct label to use
        switch(Location)
        {
            case PortLocation.Left:
                Label = LabelLeft;
                data = Opcion.Data;
                break;
            case PortLocation.Right:
                Label = LabelRight;
                data = Opcion.DataSecundaria;
                break;
        }

        switch(type)
        {
            case PortType.Text:
                Label.text = data;
                break;
            case PortType.Image:
                Label.text = "Cargando";

                //Clear escaped strings
                data = data.Replace("\\", "");

                //Request a download
                BeginLoadPortImage(data, Location);

                break;
            default:
                break;
        }
    }

    private PortType GetPortTypeForLocation(PortLocation location, Recurso.OpcionContenido opcion)
    {
        string[] types = opcion.Tipo.Split("|".ToCharArray());

        //Should have more than 1 index
        if (types.Length <= 1)
            return PortType.Unknown;

        string[] portTypes = types[1].Split("-".ToCharArray());

        //Same here, for this specific type of render option, each port type is separated via "-" like this: pares|txt-img
        if (portTypes.Length <= 1)
            return PortType.Unknown;

        int index = location == PortLocation.Left ? 0 : 1;
        return portTypes[index].Equals("txt") ? PortType.Text : portTypes[index].Equals("img") ? PortType.Image : PortType.Unknown;
    }

    private void BeginLoadPortImage(string url, PortLocation Location)
    {
        StartCoroutine(LoadImage(url, Location));
    }

    private IEnumerator LoadImage(string ImageUrl, PortLocation Location)
    {
        Debug.Log("Starting Laod Image from url: " + ImageUrl + " on port location: " + Location);
        using (WWW www = new WWW(ImageUrl))
        {
            yield return www;

            if (www.error != null)
            {
                Debug.LogError(www.error);
            }
            else
            {
                FinishLoadImage(www.texture, Location);
            }
        }
    }

    private void FinishLoadImage(Texture2D Tex, PortLocation Location)
    {
        UnityEngine.UI.Image Img = Location == PortLocation.Left ? ImageLeft : ImageRight;
        Img.overrideSprite = Sprite.Create(Tex, new Rect(0, 0, Tex.width, Tex.height), new Vector2(0, 0));

        //Remember to hide the text showing "Loading"
        UnityEngine.UI.Text Label = Location == PortLocation.Left ? LabelLeft : LabelRight;
        Label.text = "";

        Debug.Log("Image Loaded: " + Tex);
    }
}
