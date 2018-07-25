using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRenderOptionFactory
{
    RenderOption[] BuildRenderOptions(Recurso.OpcionContenido[] Options);
}

public abstract class RenderOption : MonoBehaviour {

    //public abstract void Assign(Recurso.OpcionContenido Option);
    public abstract IRenderOptionFactory GetFactory();

    //TODO: GetData deberia devolver un objeto tipo Respuesta
}
