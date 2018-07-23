using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RenderOption : MonoBehaviour {
    
    public abstract void Assign(Recurso.OpcionContenido Option);
    
    //TODO: GetData deberia devolver un objeto tipo Respuesta
}
