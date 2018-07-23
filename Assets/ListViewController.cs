using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListViewController : MonoBehaviour {

    private List<GameObject> ListItems;

    private void Start()
    {
        ListItems = new List<GameObject>();
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
        li.transform.parent = transform;


        li.transform.localPosition = Vector3.zero;
        li.transform.localScale = Vector3.one;
        li.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
