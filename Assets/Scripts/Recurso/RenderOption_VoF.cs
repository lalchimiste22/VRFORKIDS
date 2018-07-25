using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderOption_VoFFactory : IRenderOptionFactory
{
    private GameObject Blueprint;

    public RenderOption_VoFFactory(GameObject InBlueprint)
    {
        if (!InBlueprint.GetComponent<RenderOption_VoF>())
        {
            Debug.LogError("Expected RenderOption_VoF component on Blueprint GameObject, not found");
        }
        else
        {
            Blueprint = InBlueprint;
        }
    }

    public RenderOption[] BuildRenderOptions(Recurso.OpcionContenido[] Options)
    {
        List<RenderOption> RenderOptions = new List<RenderOption>();

        foreach (Recurso.OpcionContenido Opt in Options)
        {
            GameObject instantiated = GameObject.Instantiate(Blueprint);
            RenderOption_VoF alternative = instantiated.GetComponent<RenderOption_VoF>();

            alternative.Assign(Opt);
            RenderOptions.Add(alternative);
        }

        return RenderOptions.ToArray();
    }
}

public class RenderOption_VoF : RenderOption {

    public UnityEngine.UI.Toggle Toggle_True;
    public  UnityEngine.UI.Toggle Toggle_False;
    public UnityEngine.UI.Text Label;

    public override IRenderOptionFactory GetFactory()
    {
        return new RenderOption_VoFFactory(this.gameObject);
    }

    public void Assign(Recurso.OpcionContenido Option)
    {
        //We should be assigned to a toggle, so we can search it and init the values
        Toggle_True.isOn = false;
        Toggle_False.isOn = false;
        Label.text = Option.Data;
    }
}
