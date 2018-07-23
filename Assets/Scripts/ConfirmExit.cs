using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmExit : MonoBehaviour {

    public ZoomInDetail ZoomDetail;

    private bool shouldWaitForConfirmation = true;

	public void CheckForExit()
    {
        if(shouldWaitForConfirmation)
        {
            //Go to the zoom detail
            ZoomDetail.ShowZoomDetail();
            shouldWaitForConfirmation = false;
        }
        else
        {
            StartCoroutine(ChangeLevel());
        }
    }

    IEnumerator ChangeLevel()
    {   //Change Level
        MyManager.Instance.cameraManager.FadeOut();
        yield return new WaitForSeconds(1);
        Application.LoadLevel("UMenu");

        yield return new WaitForEndOfFrame();
        MyManager.Instance.cameraManager.FadeIn();

    }

    public void AbortExit()
    {
        ZoomDetail.ExitZoomDetail();
        shouldWaitForConfirmation = true;
    }
}
