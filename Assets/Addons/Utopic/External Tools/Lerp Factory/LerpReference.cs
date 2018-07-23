using UnityEngine;
using System.Collections;
namespace LerpFactory
{
	public class LerpReference : MonoBehaviour 
	{
		public LerpFactoryScript Manager;


		public Transform ObjectMoving;
		public Transform[] Transforms;
		public float[] Times;
		public TypeOfLerp TypeHere;
		public bool Repeat;
		//private bool LocalRewinding;
	//	private bool isRewinding;

		public Vector3 StartPos;
		public Quaternion StartRot;

		private bool ScriptIsLerping;
		private float timer;
		public int CurrentLerpNumber;


		private int I = 1;

		void Update () 
		{
			if(ScriptIsLerping == true)//Is this script clear to lerp?
			{
				timer += Time.deltaTime;


				if(TypeHere == TypeOfLerp.cosLerp)
				{
					if(CurrentLerpNumber > -1)
					{
						ObjectMoving.position = ActualLerp(Transforms[CurrentLerpNumber + I].position,Transforms[CurrentLerpNumber].position,timer/Times[CurrentLerpNumber + I]);
						ObjectMoving.rotation = ActualLerp(Transforms[CurrentLerpNumber + I].rotation,Transforms[CurrentLerpNumber].rotation,timer/Times[CurrentLerpNumber + I]);
					}
					else
					{
						ObjectMoving.position = ActualLerp(Transforms[0].position,StartPos,timer/Times[0]);
						ObjectMoving.rotation = ActualLerp(Transforms[0].rotation,StartRot,timer/Times[0]);
					}
				}
				else
				{
					if(CurrentLerpNumber > -1)
					{
						ObjectMoving.position = ActualLerp(Transforms[CurrentLerpNumber].position,Transforms[CurrentLerpNumber + I].position,timer/Times[CurrentLerpNumber + I]);
						ObjectMoving.rotation = ActualLerp(Transforms[CurrentLerpNumber].rotation,Transforms[CurrentLerpNumber + I].rotation,timer/Times[CurrentLerpNumber + I]);
					}
					else
					{
						ObjectMoving.position = ActualLerp(StartPos,Transforms[0].position,timer/Times[0]);
						ObjectMoving.rotation = ActualLerp(StartRot,Transforms[0].rotation,timer/Times[0]);
					}
				}

				CheckEnd();
			}
		}




		public void MapTransforms(Transform ObjectToMove,Transform[] T,float Time,bool useSpeed,TypeOfLerp Type,bool repeat)
		{
			Transforms = new Transform[T.Length];
			Times = new float[T.Length];
			ObjectMoving = ObjectToMove;
			TypeHere = Type;
			Repeat = repeat;

			StartPos = ObjectToMove.position;
			StartRot = ObjectToMove.rotation;

			for (int i = 0; i < T.Length; i++) 
			{
				Transforms[i] = T[i];
				if(useSpeed == true)
				{
					if(i > 0)
					{
						Times[i] = (Vector3.Distance(Transforms[i].position,Transforms[i-1].position))/Time;
					}
					else
					{
						Times[i] = (Vector3.Distance(Transforms[i].position,ObjectMoving.position))/Time;
					}
				}
				else
				{
					Times[i] = Time;
				}
			}

			StartLerp();
		}








		void StartLerp()
		{
			ScriptIsLerping = true;
			timer = 0f;
			I = 1;
		}



		void Reset()
		{
			if(Manager != null)
			{
				Manager.ResetLerpRef(this);//Add this script to the inactive list.
			}
		}


		void CheckEnd()
		{

			if(CurrentLerpNumber > -1)
			{
				if(ObjectMoving.position == Transforms[CurrentLerpNumber + I].position || timer/Times[CurrentLerpNumber + I] >= 1f)
				{
					CurrentLerpNumber++;//Advance to the next number.
					timer = 0f;//Reset timer.
				}
			}
			else
			{
				if(ObjectMoving.position == Transforms[0].position || timer/Times[0] >= 1f)
				{
					CurrentLerpNumber++;//Advance to the next number.
					timer = 0f;//Reset timer.
				}
			}



			if(Transforms.Length - 1 == CurrentLerpNumber)
			{
				if(Repeat == true)
				{
					CurrentLerpNumber = -1;
					timer = 0f;//Reset timer.
					//This means that the object will cycle for ever.
				}
				else
				{
					ScriptIsLerping = false;//Stop the lerp.
					Reset();//Reset.
				}
			}
		}







		Vector3 ActualLerp(Vector3 S,Vector3 E,float T)
		{
			switch(TypeHere)//This switch block is for setting the percentage/time parameter.
			{
			case TypeOfLerp.Lerp://T is not set to anything special as this is the normal lerp.
				break;
			case TypeOfLerp.SmoothStep:
				T = T*T*(3-2*T);//This will lerp by a smooth step curve.
				break;
			case TypeOfLerp.SmootherStep:
				T = T*T*T * (T * (6f*T - 15f) + 10f);//This will lerp by a smoother step curve.
				break;
			case TypeOfLerp.Expo:
				T = T*T;//This will lerp by an exponential curve.
				break;
			case TypeOfLerp.InverseExpo:
				T = (1-T)*(1-T);//This will lerp by an inverse exponential curve.
				break;
			case TypeOfLerp.sinLerp:
				T = Mathf.Sin(T * Mathf.PI * 0.5f);//This will ease out the lerp.
				break;
			case TypeOfLerp.cosLerp:
				T = Mathf.Cos(T * Mathf.PI * 0.5f);//This will ease in the lerp.
				break;
			default:
				break;
			}

			if(TypeHere != TypeOfLerp.InverseExpo)//In order for the inverse exponential lerp the start and end values must be reversed, hence the similer code.
			{
				return(Vector3.Lerp(S,E,T));//Lerp the vector3 output.
			}
			else
			{
				return(Vector3.Lerp(E,S,T));//Lerp the vector2 output.
			}
		}

		Quaternion ActualLerp(Quaternion S,Quaternion E,float T)
		{
			switch(TypeHere)//This switch block is for setting the percentage/time parameter.
			{
			case TypeOfLerp.Lerp://T is not set to anything special as this is the normal lerp.
				break;
			case TypeOfLerp.SmoothStep:
				T = T*T*(3-2*T);//This will lerp by a smooth step curve.
				break;
			case TypeOfLerp.SmootherStep:
				T = T*T*T * (T * (6f*T - 15f) + 10f);//This will lerp by a smoother step curve.
				break;
			case TypeOfLerp.Expo:
				T = T*T;//This will lerp by an exponential curve.
				break;
			case TypeOfLerp.InverseExpo:
				T = (1-T)*(1-T);//This will lerp by an inverse exponential curve.
				break;
			case TypeOfLerp.sinLerp:
				T = Mathf.Sin(T * Mathf.PI * 0.5f);//This will ease out the lerp.
				break;
			case TypeOfLerp.cosLerp:
				T = Mathf.Cos(T * Mathf.PI * 0.5f);//This will ease in the lerp.
				break;
			default:
				break;
			}

			if(TypeHere != TypeOfLerp.InverseExpo)//In order for the inverse exponential lerp the start and end values must be reversed, hence the similer code.
			{
				return(Quaternion.Lerp(S,E,T));//Lerp the vector3 output.
			}
			else
			{
				return(Quaternion.Lerp(E,S,T));//Lerp the vector2 output.
			}
		}
	}
}
