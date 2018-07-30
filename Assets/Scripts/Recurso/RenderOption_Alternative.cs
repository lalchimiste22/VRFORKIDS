using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderOption_AlternativeFactory : IRenderOptionFactory
{
    private GameObject Blueprint;

    public RenderOption_AlternativeFactory(GameObject InBlueprint)
    {
        if(!InBlueprint.GetComponent<RenderOption_Alternative>())
        {
            Debug.LogError("Expected RenderOption_Alternative component on Blueprint GameObject, not found");
        }
        else
        {
            Blueprint = InBlueprint;
        }
    }

    public RenderOption[] BuildRenderOptions(Recurso.OpcionContenido[] Options)
    {
        List<RenderOption> RenderOptions = new List<RenderOption>();

        for(int i = 0; i < Options.Length; i++)
        {
            Recurso.OpcionContenido Opt = Options[i];
            GameObject instantiated = GameObject.Instantiate(Blueprint);
            RenderOption_Alternative alternative = instantiated.GetComponent<RenderOption_Alternative>();

            alternative.Assign(Opt, i);
            RenderOptions.Add(alternative);
        }

        return RenderOptions.ToArray();
    }
}

public class RenderOption_Alternative : RenderOption {

    public UnityEngine.UI.Toggle Toggle;
    public UnityEngine.UI.Text Label;
    public UnityEngine.UI.Text IndexLabel;

    public void Assign(Recurso.OpcionContenido Option, int index)
    {
        //We should be assigned to a toggle, so we can search it and init the values
        Toggle.isOn = false;
        Label.text = Option.Data;
        int asciiValue = (int)'A' + index;
        IndexLabel.text = ((char)asciiValue).ToString();
    }

    public override IRenderOptionFactory GetFactory()
    {
        return new RenderOption_AlternativeFactory(this.gameObject);
    }

}
