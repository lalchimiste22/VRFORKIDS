using UnityEngine;
using System.Collections;
using LerpFactory;

public class CameraTest : MonoBehaviour 
{

	public Transform ObjectToMove;
	public Transform[] TransformsToLerpTo;


	public float MinFOV,MaxFOV;
	public float MinTimeToLerp,MaxTimeToLerp;
	public float MinTimeTillAdjust,MaxTimeTillAdjust;

	private float timer,timetowaitto;

	public Light light;
	public Light light2;
	private float timer2;
	private float time2 = 2f;

	public Color startColour,endColour;

	public int TestingInt;
	public float TestingFloat;
	public double TestingDouble;
	public Vector2 TestingVector2;
	public Vector3 TestingVector3;
	public Vector4 TestingVector4;
	public Quaternion TestingQuaternion;
	public Color TestingColor;
	public bool TestBool;


	public Transform test;



	/************************************************/
	public LerpFactoryScript LFS;
	/***********************************************/


	void Start ()
	{
		timetowaitto = Random.Range(MinTimeTillAdjust,MaxTimeTillAdjust);

		LFS.LerpRefTransforms(ObjectToMove,TransformsToLerpTo,10f,true,TypeOfLerp.Expo,true);
		//LFS.LerpRefTransform(ObjectToMove,TransformsToLerpTo[1].gameObject.transform,10f,true,TypeOfLerp.sinLerp,true,true);

		LFS.Bool((x)=>TestBool=x,false,true,1f);

		LFS.Float((x)=>TestingFloat=x,0f,10f,1f,false,0f,0f,LerpMode.Normal,TypeOfLerp.Expo,0f);
	}
	
	void Update () 
	{
		timer += Time.deltaTime;//Increase timer.
		timer2 += Time.deltaTime;//Increase timer2.
		print(timer + "   " + timetowaitto);
		if(timer >= timetowaitto)
		{
			LFS.Float((x)=>light.intensity=x,0f,8f,0.25f,false,0f,0f,LerpMode.Rewind,TypeOfLerp.Lerp,0.25f);//This for the white light.
			LFS.Float((x)=>light.range=x,0f,10f,0.25f,false,0f,0f,LerpMode.Rewind,TypeOfLerp.Lerp,0.25f);//This for the white light.

			//This for the camera field of view.
			timetowaitto = Random.Range(MinTimeTillAdjust,MaxTimeTillAdjust);//Sets the trip value to a new value.
			LFS.Float((x)=>gameObject.GetComponent<Camera>().fieldOfView=x,gameObject.GetComponent<Camera>().fieldOfView,Random.Range(MinFOV,MaxFOV),timetowaitto,false,1f,0f,LerpMode.Normal,TypeOfLerp.Lerp,0f);
			timer = 0f;//reset the timer.
		}



		if(timer2 >= time2)//This block is for the flickering blue light.
		{

			if(light2.intensity >= 2.5f)//This is a simple way of checking if the lerp has finished. As there is no FlickerRewind we have to do it by hand.
			{
				LFS.Float((x)=>light2.intensity=x,4.5f,0.5f,2f,0.5f,-0.5f);
				LFS.Colour((x)=>light2.color=x,endColour,startColour,1.9f);
			}
			else
			{
				LFS.Float((x)=>light2.intensity=x,0.5f,4.5f,2f,0.5f,-0.5f);
				LFS.Colour((x)=>light2.color=x,startColour,endColour,1.9f);
			}
			timer2 =0f;//Reset timer2.
		}
	}
}
