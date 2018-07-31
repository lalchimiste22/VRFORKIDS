using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeLevelDirector : MonoBehaviour {

    public EaseMoveTo Bee;
    public Transform BeeFlowerTarget;
    public Transform FlowerPosition;
    public Recurso FlowerResource;



	public void BeginFlowerCinematic()
    {
        MyManager.Instance.FadeOut(AfterFadeoutContinueFlowerCinematic);

    }

    private void AfterFadeoutContinueFlowerCinematic()
    {
        //First move the character
        MyManager.Instance.playerGameObject.transform.position = FlowerPosition.position;
        MyManager.Instance.playerGameObject.transform.eulerAngles = FlowerPosition.eulerAngles;

        //Second start the fade in
        MyManager.Instance.FadeIn();

        //Third, begin the smooth movement on the bee
        Bee.SmoothTransport(new Trans(BeeFlowerTarget), -1.0f, OnBeeReachedFlower);
    }

    private void OnBeeReachedFlower()
    {
        FlowerResource.Show();
    }
}
