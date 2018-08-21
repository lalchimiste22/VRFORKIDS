using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInDetail : MonoBehaviour {

    //Player to move
    public EaseMoveTo PlayerObject;
    
    //The position to move to
    public Transform TargetPosition;

    //The transform to go back to
    public Transform ExitTransform;

    //Exit point
    private Trans ExitPoint;

    public void Start()
    {
        if(!PlayerObject)
            PlayerObject = MyManager.Instance.playerGameObject.GetComponent<EaseMoveTo>();

        if (!PlayerObject)
            Debug.LogError("No player object with EaseMovment found");

    }

    public void ShowZoomDetail()
    {
        if (ExitTransform == null)
            ExitPoint = new Trans(MyManager.Instance.playerGameObject.transform);
        else
            ExitPoint = new Trans(ExitTransform);

        PlayerObject.SmoothTransport(new Trans(TargetPosition));
    }

    public void ExitZoomDetail()
    {
        PlayerObject.SmoothTransport(ExitPoint);
    }
}
