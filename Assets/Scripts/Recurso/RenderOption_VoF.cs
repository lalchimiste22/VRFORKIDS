using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderOption_VoF : RenderOption {

    public UnityEngine.UI.Toggle Toggle_True;
    public  UnityEngine.UI.Toggle Toggle_False;
    public UnityEngine.UI.Text Label;
    

    public override void Assign(Recurso.OpcionContenido Option)
    {
        //We should be assigned to a toggle, so we can search it and init the values
        Toggle_True.isOn = false;
        Toggle_False.isOn = false;
        Label.text = Option.Data;
    }
}
