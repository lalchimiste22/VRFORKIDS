using UnityEngine;
using System.Collections;

public class LevelSwitch : MonoBehaviour {

	public Transform playerCamera;
	public Transform labPoint;
	public Transform infoPoint;
	public CoinMovement Rotator;
	public GameObject Lights;

	[System.Serializable] public struct OrganInfo
	{
		public GameObject Organ;
		public GameObject UI;
	}
	public OrganInfo[] Organs;

	private Transform player;
	private MyCameraManager cameraManager;

	void Start () {
		player = MyManager.Instance.playerGameObject.transform;
		cameraManager = MyManager.Instance.cameraManager;
	}



	public void MovePlayerToInfoPoint()
	{
		cameraManager.FadeOut ();
		Invoke ("_MovePlayerToInfoPoint", cameraManager.fadeOutTime);
	}
	private void _MovePlayerToInfoPoint()
	{
		player.position = infoPoint.position;
		infoPoint.rotation = playerCamera.rotation;
		Rotator.enabled = true;
		Rotator.Restart ();
        if(Lights != null)
		    Lights.SetActive (false);
		cameraManager.FadeIn ();
	}




	public void MovePlayerToLabPoint()
	{
		cameraManager.FadeOut ();
		Invoke ("_MovePlayerToLabPoint", cameraManager.fadeOutTime);
	}
	private void _MovePlayerToLabPoint()
	{
		player.position = labPoint.position;
		Rotator.enabled = false;

		foreach ( OrganInfo obj in Organs) {
			obj.Organ.SetActive (false);
			obj.UI.SetActive (false);
		}

        if (Lights != null)
            Lights.SetActive (true);
		cameraManager.FadeIn ();
	}
    
}
