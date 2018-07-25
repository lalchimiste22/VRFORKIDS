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
    
    public void SetupPort(PortLocation Location, Recurso.OpcionContenido opcion)
    {
        //For now, just setup the name
        switch(Location)
        {
            case PortLocation.Left:
                LabelLeft.text = opcion.Data;
                break;
            case PortLocation.Right:
                LabelRight.text = opcion.DataSecundaria;
                break;
        }
    }
}
