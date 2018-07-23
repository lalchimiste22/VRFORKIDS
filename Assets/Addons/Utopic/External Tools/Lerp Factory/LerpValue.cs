using UnityEngine;
using System.Collections;

namespace LerpFactory
{
	public class LerpValue : MonoBehaviour 
	{
		//This is for internal useage.
		private int[] I = new int[4];
		private float[] F = new float[4];
		private double[] D = new double[4];
		private Vector2[] V2 = new Vector2[4];
		private Vector3[] V3 = new Vector3[4];
		private Vector4[] V4 = new Vector4[4];
		private Quaternion[] Q = new Quaternion[4]{Quaternion.Euler(Vector3.one * 360),Quaternion.identity,Quaternion.identity,Quaternion.identity};//We set the defaults in this case to stop an error message that appears when we try to lerp from Quaternion.identity to Quaternion.identity.
		private Color[] C = new Color[4];

		private TypeOfLerp CurrentType;
		private LerpMode CurrentMode;

		//This is for the actual lerp.
		float CurrentTime;
		float StartTime;
		float EndTime;
		float TimeTillRewind;
		float timeToTake;
		float T;

		public LerpFactoryScript Manager;
		private int outInt;
		private float outFloat;
		private double outDouble;
		private Vector2 outVector2;
		private Vector3 outVector3;
		private Vector4 outVector4;
		private Quaternion outQuaternion;
		private Color outColour;






		public void SetOtherNumbers(float time,float rewind,TypeOfLerp T,LerpMode M,bool callback)
		{
			timeToTake = time;//Map the amount of time to take.
			TimeTillRewind = rewind;//Map the amount of time till starting the rewind.
			CurrentType = T;//Map the type of lerp.
			CurrentMode = M;//Map the mode of the lerp.
		}



		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\

		public void setInt(int i0,int i1,int i2,int i3)//This is for internal useage. It is used at the start of the lerp to map the input data to the actual lerp object.
		{
			I[0] = i0;		I[1] = i1;		I[2] = i2;		I[3] = i3;
		}

		public void setFloat(float i0,float i1,float i2,float i3)//This is for internal useage.
		{
			F[0] = i0;		F[1] = i1;		F[2] = i2;		F[3] = i3;
		}

		public void setDouble(double i0,double i1,double i2,double i3)//This is for internal useage. It is used at the start of the lerp to map the input data to the actual lerp object.
		{
			D[0] = i0;		D[1] = i1;		D[2] = i2;		D[3] = i3;
		}

		public void setV2(Vector2 i0,Vector2 i1,Vector2 i2,Vector2 i3)//This is for internal useage. It is used at the start of the lerp to map the input data to the actual lerp object.
		{
			V2[0] = i0;		V2[1] = i1;		V2[2] = i2;		V2[3] = i3;
		}

		public void setV3(Vector3 i0,Vector3 i1,Vector3 i2,Vector3 i3)//This is for internal useage. It is used at the start of the lerp to map the input data to the actual lerp object.
		{
			V3[0] = i0;		V3[1] = i1;		V3[2] = i2;		V3[3] = i3;
		}

		public void setV4(Vector4 i0,Vector4 i1,Vector4 i2,Vector4 i3)//This is for internal useage. It is used at the start of the lerp to map the input data to the actual lerp object.
		{
			V4[0] = i0;		V4[1] = i1;		V4[2] = i2;		V4[3] = i3;
		}

		public void setC(Color i0,Color i1,Color i2,Color i3)//This is for internal useage. It is used at the start of the lerp to map the input data to the actual lerp object.
		{
			C[0] = i0;		C[1] = i1;		C[2] = i2;		C[3] = i3;
		}

		public void setQ(Quaternion i0,Quaternion i1)//This is for internal useage. It is used at the start of the lerp to map the input data to the actual lerp object.
		{
			Q[0] = i0;		Q[1] = i1;//There are only two inputs as one cannot add Quaternion. So it is impossible to have a flicker effect.
		}

		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\





		private void Reset()//This function contains the 
		{
			if(Manager != null)
			{
				Manager.ResetLerp(this);//Add this script to the inactive list.
			}
		}
			

		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\

		public IEnumerator LerpBool//This is the contains the actual code that handles the lerp.
		(System.Action<bool> urB,bool sv,bool ev)
		{
			StartTime = Time.time;//Start Time of the lerp.
			EndTime = StartTime + timeToTake;//End Time of the lerp.

			while (Time.time < EndTime)//Has the lerp Finished?
			{
				CurrentTime = (Time.time-StartTime)/timeToTake;//Set lerp t variable.
				yield return null;//Wait until the end of the frame;
			}
				
			if(CurrentMode == LerpMode.Rewind)//Branch for Rewind.
			{
				urB(ev);

				EndTime = Time.time + Mathf.Abs(TimeTillRewind);//The reason TimeTillRewind is abs is because the user may input a negative number. That would break the code.

				while (Time.time < EndTime)//This is the pause before the rewind.
				{
					yield return null;//Wait until the end of the frame;
				}

				StartTime = Time.time;//Start Time of the rewind.
				EndTime = StartTime + timeToTake;//End Time of the rewind.

				while (Time.time < EndTime)//The rewind.
				{
					CurrentTime = (EndTime-Time.time)/timeToTake;//Set lerp t variable.
					yield return null;//Wait until the end of the frame;
				}
			}

			if(CurrentMode != LerpMode.Rewind)//Is the lerpmode set to rewind?
			{
				urB(ev);
			}
			else
			{
				urB(sv);
			}

			Reset();//Run the reset code.

		}

		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\

		public IEnumerator Lerp//This is the contains the actual code that handles the lerp.
		(
			System.Action<int> urI,
			System.Action<float> urF,
			System.Action<double> urD,
			System.Action<Vector2> urV2,
			System.Action<Vector3> urV3,
			System.Action<Vector4> urV4,
			System.Action<Color> urC,
			System.Action<Quaternion> urQ
		)
		{
			StartTime = Time.time;//Start Time of the lerp.
			EndTime = StartTime + timeToTake;//End Time of the lerp.

			while (Time.time < EndTime)//Has the lerp Finished?
			{
				CurrentTime = (Time.time-StartTime)/timeToTake;//Set lerp t variable.
				ActualLerp();//This does the lerp and updates the values.

				if(urI != null){urI((outInt));}//The actual lerp.
				if(urF != null){urF((outFloat));}//The actual lerp.
				if(urD != null){urD((outDouble));}//The actual lerp.
				if(urV2 != null){urV2((outVector2));}//The actual lerp.
				if(urV3 != null){urV3((outVector3));}//The actual lerp.
				if(urV4 != null){urV4((outVector4));}//The actual lerp.
				if(urC != null){urC((outColour));}//The actual lerp.
				if(urQ != null){urQ((outQuaternion));}//The actual lerp.

				yield return null;//Wait until the end of the frame;
			}

			if(CurrentMode == LerpMode.Rewind)//Branch for Rewind.
			{
				//We do not run RewindCoroutine as that would mean losing the reference to the data to change.

				if(urI != null){urI(I[1]);}//The last of the lerp steps.
				if(urF != null){urF(F[1]);}//The last of the lerp steps.
				if(urD != null){urD(D[1]);}//The last of the lerp steps.
				if(urV2 != null){urV2(V2[1]);}//The last of the lerp steps.
				if(urV3 != null){urV3(V3[1]);}//The last of the lerp steps.
				if(urV4 != null){urV4(V4[1]);}//The last of the lerp steps.
				if(urC != null){urC(C[1]);}//The last of the lerp steps.
				if(urQ != null){urQ(Q[1]);}//The last of the lerp steps.

				EndTime = Time.time + Mathf.Abs(TimeTillRewind);//The reason TimeTillRewind is abs is because the user may input a negative number. That would break the code.

				while (Time.time < EndTime)//This is the pause before the rewind.
				{
					yield return null;//Wait until the end of the frame;
				}

				StartTime = Time.time;//Start Time of the rewind.
				EndTime = StartTime + timeToTake;//End Time of the rewind.

				while (Time.time < EndTime)//The rewind.
				{
					CurrentTime = (EndTime-Time.time)/timeToTake;//Set lerp t variable.
					ActualLerp();//This does the lerp and updates the values.

					if(urI != null){urI((outInt));}//The actual lerp.
					if(urF != null){urF((outFloat));}//The actual lerp.
					if(urD != null){urD((outDouble));}//The actual lerp.
					if(urV2 != null){urV2((outVector2));}//The actual lerp.
					if(urV3 != null){urV3((outVector3));}//The actual lerp.
					if(urV4 != null){urV4((outVector4));}//The actual lerp.
					if(urC != null){urC((outColour));}//The actual lerp.
					if(urQ != null){urQ((outQuaternion));}//The actual lerp.

					yield return null;//Wait until the end of the frame;
				}
			}

			if(CurrentMode != LerpMode.Rewind)//Is the lerpmode set to rewind?
			{
				if(urI != null){urI(I[1]);}//The last of the lerp steps.
				if(urF != null){urF(F[1]);}//The last of the lerp steps.
				if(urD != null){urD(D[1]);}//The last of the lerp steps.
				if(urV2 != null){urV2(V2[1]);}//The last of the lerp steps.
				if(urV3 != null){urV3(V3[1]);}//The last of the lerp steps.
				if(urV4 != null){urV4(V4[1]);}//The last of the lerp steps.
				if(urC != null){urC(C[1]);}//The last of the lerp steps.
				if(urQ != null){urQ(Q[1]);}//The last of the lerp steps.
			}
			else
			{
				if(urI != null){urI(I[0]);}//The last of the lerp steps.
				if(urF != null){urF(F[0]);}//The last of the lerp steps.
				if(urD != null){urD(D[0]);}//The last of the lerp steps.
				if(urV2 != null){urV2(V2[0]);}//The last of the lerp steps.
				if(urV3 != null){urV3(V3[0]);}//The last of the lerp steps.
				if(urV4 != null){urV4(V4[0]);}//The last of the lerp steps.
				if(urC != null){urC(C[0]);}//The last of the lerp steps.
				if(urQ != null){urQ(Q[0]);}//The last of the lerp steps.
			}

			Reset();//Run the reset code.
		}

		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\





		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\

		void ActualLerp()
		{
			T = CurrentTime;

			switch(CurrentType)//This switch block is for setting the percentage/time parameter.
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

			if(CurrentType != TypeOfLerp.InverseExpo)//In order for the inverse exponential lerp the start and end values must be reversed, hence the similer code.
			{
				outInt = (int)(Mathf.Lerp(I[0],I[1],T)) + Random.Range(I[2],I[3]);//Lerp the int output.
				outFloat = (Mathf.Lerp(F[0],F[1],T)) + Random.Range(F[2],F[3]);//Lerp the float output.
				outDouble = (double)(Mathf.Lerp((float)D[0],(float)D[1],T)) + Random.Range((float)D[2],(float)D[3]);//Lerp the double output.
				outVector2 = Vector2.Lerp(V2[0],V2[1],T) + Ran2V2(V2[2],V2[3]);//Lerp the vector2 output.
				outVector3 = Vector3.Lerp(V3[0],V3[1],T) + Ran2V3(V3[2],V3[3]);//Lerp the vector3 output.
				outVector4 = Vector4.Lerp(V4[0],V4[1],T) + Ran2V4(V4[2],V4[3]);//Lerp the vector4 output.
				outQuaternion = Quaternion.Lerp(Q[0],Q[1],T);//Lerp the quaternion output.
				outColour = Color.Lerp(C[0],C[1],T) + Ran2C(C[2],C[3]);//Lerp the colour output.
			}
			else
			{
				outInt = (int)(Mathf.Lerp(I[1],I[0],T)) + Random.Range(I[2],I[3]);//Lerp the int output.
				outFloat = (Mathf.Lerp(F[1],F[0],T)) + Random.Range(F[2],F[3]);//Lerp the float output.
				outDouble = (double)(Mathf.Lerp((float)D[1],(float)D[0],T)) + Random.Range((float)D[2],(float)D[3]);//Lerp the double output.
				outVector2 = Vector2.Lerp(V2[1],V2[0],T) + Ran2V2(V2[2],V2[3]);//Lerp the vector2 output.
				outVector3 = Vector3.Lerp(V3[1],V3[0],T) + Ran2V3(V3[2],V3[3]);//Lerp the vector2 output.
				outVector4 = Vector4.Lerp(V4[1],V4[0],T) + Ran2V4(V4[2],V4[3]);//Lerp the vector2 output.
				outQuaternion = Quaternion.Lerp(Q[1],Q[0],T);//Lerp the quaternion output.
				outColour = Color.Lerp(C[1],C[0],T) + Ran2C(C[2],C[3]);//Lerp the colour output.
			}
		}

		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\
		//##################################################################################################################################################################################################\\












		//These functions are for randomizing purposes only.

		Vector2 Ran2V2(Vector2 V1,Vector2 V2)//Produce a random vector2.
		{
			Vector2 Temp;
			Temp = new Vector2(Random.Range(V1.x,V2.x),Random.Range(V1.y,V2.y));//Get the random vector.
			return(Temp);
		}


		Vector3 Ran2V3(Vector3 V1,Vector3 V2)//Produce a random vector3.
		{
			Vector3 Temp;
			Temp = new Vector3(Random.Range(V1.x,V2.x),Random.Range(V1.y,V2.y),Random.Range(V1.z,V2.z));//Get the random vector.
			return(Temp);
		}


		Vector4 Ran2V4(Vector4 V1,Vector4 V2)//Produce a random vector4.
		{
			Vector4 Temp;
			Temp = new Vector4(Random.Range(V1.x,V2.x),Random.Range(V1.y,V2.y),Random.Range(V1.z,V2.z),Random.Range(V1.w,V2.w));//Get the random vector.
			return(Temp);
		}

		Color Ran2C(Color C1,Color C2)//Produce a random colour.
		{
			Color Temp;
			Temp = new Color(Random.Range(C1.a,C2.a),Random.Range(C1.r,C2.r),Random.Range(C1.g,C2.g),Random.Range(C1.b,C2.b));//Get the random colour.
			return(Temp);
		}







	}
}
