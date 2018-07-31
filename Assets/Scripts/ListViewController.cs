using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListViewController : MonoBehaviour {

    private List<GameObject> ListItems = new List<GameObject>();
    public RectTransform Content;
    public UnityEngine.UI.ScrollRect ScrollRect;
    public float ScrollVelocity = 1.0f;

    public UnityEngine.UI.Button DownButton;
    public UnityEngine.UI.Button UpButton;

    private bool bNeedsScroll = false;
    private RectTransform ScrollRectTransform;

    private void Awake()
    {
        //ListItems = new List<GameObject>();
        ScrollRectTransform = ScrollRect.GetComponent<RectTransform>();
    }

    public void Clear()
    {
        foreach(GameObject go in ListItems)
        {
            Destroy(go);
        }

        ListItems.Clear();
    }

    private void Update()
    {
        if(bNeedsScroll)
        {
            UpButton.gameObject.SetActive(ScrollRect.verticalNormalizedPosition < 1.0f);
            DownButton.gameObject.SetActive(ScrollRect.verticalNormalizedPosition > 0.0f);
        }
        else
        {
            UpButton.gameObject.SetActive(false);
            DownButton.gameObject.SetActive(false);
        }
    }

    public void Add(GameObject li)
    {
        ListItems.Add(li);
        li.transform.parent = Content.transform;

        li.transform.localPosition = Vector3.zero;
        li.transform.localScale = Vector3.one;
        li.transform.localRotation = Quaternion.Euler(Vector3.zero);

        RecalculateRequirements();
    }

    public void ScrollUp()
    {
        ScrollRect.verticalNormalizedPosition += GetScrollDelta();

        if (ScrollRect.verticalNormalizedPosition >= 1.0f)
            ScrollRect.verticalNormalizedPosition = 1.0f;
    }

    public void ScrollDown()
    {
        ScrollRect.verticalNormalizedPosition -= GetScrollDelta();

        if(ScrollRect.verticalNormalizedPosition <= 0)
            ScrollRect.verticalNormalizedPosition = 0.0f;
    }

    private float GetScrollDelta()
    {
        return ScrollVelocity * Time.deltaTime * Mathf.Abs(ScrollRectTransform.sizeDelta.y / Content.sizeDelta.y);
    }

    private void RecalculateRequirements()
    {
        bNeedsScroll = Content.sizeDelta.y > ScrollRectTransform.sizeDelta.y;
    }
}
