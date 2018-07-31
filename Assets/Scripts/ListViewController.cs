using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListViewController : MonoBehaviour {

    private List<GameObject> ListItems = new List<GameObject>();
    public RectTransform Content;
    public UnityEngine.UI.ScrollRect ScrollRect;

    private void Start()
    {
        //ListItems = new List<GameObject>();
    }

    public void Clear()
    {
        foreach(GameObject go in ListItems)
        {
            Destroy(go);
        }

        ListItems.Clear();
    }

    public void Add(GameObject li)
    {
        ListItems.Add(li);
        li.transform.parent = Content.transform;

        li.transform.localPosition = Vector3.zero;
        li.transform.localScale = Vector3.one;
        li.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void ScrollUp()
    {
        ScrollRect.verticalNormalizedPosition = 1;
    }

    public void ScrollDown()
    {
        ScrollRect.verticalNormalizedPosition = 0;
    }
}
