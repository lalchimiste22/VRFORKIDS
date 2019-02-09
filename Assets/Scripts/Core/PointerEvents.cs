using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private bool bPointerInside = false;
    public UnityEvent OnPointerStay;

    void OnDisable()
    {
        bPointerInside = false;
    }

    void Update()
    {
        if (bPointerInside && OnPointerStay != null)
            OnPointerStay.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bPointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bPointerInside = false;
    }

}
