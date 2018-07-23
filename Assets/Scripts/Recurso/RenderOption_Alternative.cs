using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderOption_Alternative : RenderOption {

    private UnityEngine.UI.Toggle Toggle;
    private UnityEngine.UI.Text Label;
    private bool bNeedsInit = true;
    
    private void Initialize()
    {
        Toggle = GetComponent<UnityEngine.UI.Toggle>();
        Label = GetComponentInChildren<UnityEngine.UI.Text>(); //Label is on a children GameObject

        if (!Toggle || !Label)
            Debug.LogError("Could not find UI elements");

        bNeedsInit = false;
    }

    public override void Assign(Recurso.OpcionContenido Option)
    {
        if (bNeedsInit)
            Initialize();

        //We should be assigned to a toggle, so we can search it and init the values
        Toggle.isOn = false;
        Label.text = Option.Data;
    }
}
