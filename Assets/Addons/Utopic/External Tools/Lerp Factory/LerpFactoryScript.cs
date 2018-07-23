using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif




namespace LerpFactory
{
	public enum TypeOfLerp{Lerp,SmoothStep,SmootherStep,sinLerp,cosLerp,Expo,InverseExpo}
	public enum LerpMode{Normal,Rewind,Flicker};//An enumeration to use when setting the type of lerp. 


	#if UNITY_EDITOR
	[CustomEditor(typeof(LerpFactoryScript))]
	public class LerpFactoryEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			LerpFactoryScript myScript = (LerpFactoryScript)target;
			if(GUILayout.Button("Build Objects"))
			{
				myScript.SpawnStartObjects();
			}
		}
	}
	#endif



	public class LerpFactoryScript : MonoBehaviour 
	{
		List<GameObject> ActiveScripts = new List<GameObject>();//The List of lerp scripts that are active.
		List<GameObject> InactiveScripts = new List<GameObject>();//The List of lerp scripts that are inactive, and hence can be used to lerp something.

		List<GameObject> ActiveRefScripts = new List<GameObject>();//The List of lerp scripts that are active.
		List<GameObject> InactiveRefScripts = new List<GameObject>();//The List of lerp scripts that are inactive, and hence can be used to lerp something.

		[HideInInspector]
		public GameObject RefLerpTemplate;
		[HideInInspector]
		public GameObject RefLerpRefTemplate;
		[HideInInspector]
		public Transform ActiveParent,UnactiveParent;//These store the helper that help to tidy the lerp scripts GameObjects.

		private LerpValue TempLerpScript;//This is used when a lerp is being started.
		private LerpReference TempLerpRefScript;//This is used when a lerp is being started.



		void Start () 
		{
			SpawnStartObjects();
		}



		public void SpawnStartObjects()
		{
			if(RefLerpTemplate == null)
			{
				if(this.transform.GetChild(0).name != "Lerp Value Script")
				{
					//This handles the actual spawning of the objects.
					RefLerpTemplate = new GameObject();
					RefLerpTemplate.transform.SetParent(this.transform);
					RefLerpTemplate.name = "Lerp Value Script";
					RefLerpTemplate.AddComponent<LerpValue>();
				}
				else
				{
					RefLerpTemplate = this.transform.GetChild(0).gameObject;
				}
			}

			if(RefLerpRefTemplate == null)
			{
				if(this.transform.GetChild(1).name != "Lerp Refenerce Script")
				{
					//This handles the actual spawning of the objects.
					RefLerpRefTemplate = new GameObject();
					RefLerpRefTemplate.transform.SetParent(this.transform);
					RefLerpRefTemplate.name = "Lerp Refenerce Script";
					RefLerpRefTemplate.AddComponent<LerpReference>();
				}
				else
				{
					RefLerpRefTemplate = this.transform.GetChild(0).gameObject;
				}
			}

			if(ActiveParent == null)
			{
				if(this.transform.GetChild(2).name != "---<ActiveLerpParent>---")
				{
					//This handles the actual spawning of the objects.
					ActiveParent = new GameObject().transform;
					ActiveParent.transform.SetParent(this.transform);
					ActiveParent.name = "---<ActiveLerpParent>---";
				}
				else
				{
					ActiveParent = this.transform.GetChild(2).transform;
				}
			}

			if(UnactiveParent == null)
			{
				if(this.transform.GetChild(3).name != "---<UnactiveLerpParent>---")
				{
					//This handles the actual spawning of the objects.
					UnactiveParent = new GameObject().transform;
					UnactiveParent.transform.SetParent(this.transform);
					UnactiveParent.name = "---<UnactiveLerpParent>---";
				}
				else
				{
					UnactiveParent = this.transform.GetChild(3).transform;
				}
			}
		}



		public void ResetLerp(LerpValue T)
		{
			if(T != null)
			{
				T.gameObject.transform.SetParent(UnactiveParent);//Set the parent transform for tidiness.
				ActiveScripts.Remove(T.gameObject);//Remove from active list.
				InactiveScripts.Add(T.gameObject);//Add to inactive list.
			}
			else
			{
				print("Failed to reset a Lerp script.");//Send error message.
			}
		}


		public void ResetLerpRef(LerpReference T)
		{
			if(T != null)
			{
				T.gameObject.transform.SetParent(UnactiveParent);//Set the parent transform for tidiness.
				ActiveRefScripts.Remove(T.gameObject);//Remove from active list.
				InactiveRefScripts.Add(T.gameObject);//Add to inactive list.
			}
			else
			{
				print("Failed to reset a Lerp script.");//Send error message.
			}
		}
			


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Bool(System.Action<bool> M,bool SV,bool EV,float Time, float RewindTime)//Everything.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.
			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,true);//Map the variable data.

			StartCoroutine(TempLerpScript.LerpBool(M,SV,EV));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Bool(System.Action<bool> M,bool SV,bool EV,float Time)//This is the absolute minimum.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.
			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,true);//Map the variable data.

			StartCoroutine(TempLerpScript.LerpBool(M,SV,EV));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Int(System.Action<int> M,int SV,int EV,float Time,bool UseSpeed,int FlickerMax,int FlickerMin,LerpMode Mode,TypeOfLerp LerpType,float RewindTime)//Everything.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs((float)SV - (float)EV)/Time;}

			if(Mode != LerpMode.Flicker)
			{
				FlickerMax = 0;
				FlickerMin = 0;
			}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,LerpType,Mode,true);//Map the variable data.
			TempLerpScript.setInt(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(M,null,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Int(System.Action<int> M,int SV,int EV,float Time,float RewindTime,bool UseSpeed)//Rewind and usespeed.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs((float)SV - (float)EV)/Time;}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setInt(SV,EV,0,0);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(M,null,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Int(System.Action<int> M,int SV,int EV,float Time,float RewindTime)//Rewind.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setInt(SV,EV,0,0);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(M,null,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Int(System.Action<int> M,int SV,int EV,float Time,bool UseSpeed)//Usespeed
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs((float)SV - (float)EV)/Time;}

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setInt(SV,EV,0,0);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(M,null,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Int(System.Action<int> M,int SV,int EV,float Time,int FlickerMax,int FlickerMin)//Flicker.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Flicker,false);//Map the variable data.
			TempLerpScript.setInt(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(M,null,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Int(System.Action<int> M,int SV,int EV,float Time)//This is the absolute minimum.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setInt(SV,EV,0,0);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(M,null,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Float(System.Action<float> M,float SV,float EV,float Time,bool UseSpeed,float FlickerMax,float FlickerMin,LerpMode Mode,TypeOfLerp LerpType,float RewindTime)//Everything.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs((float)SV - (float)EV)/Time;}

			if(Mode != LerpMode.Flicker)
			{
				FlickerMax = 0f;
				FlickerMin = 0f;
			}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,LerpType,Mode,true);//Map the variable data.
			TempLerpScript.setFloat(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,M,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Float(System.Action<float> M,float SV,float EV,float Time,float RewindTime,bool UseSpeed)//Rewind and usespeed.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs((float)SV - (float)EV)/Time;}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setFloat(SV,EV,0f,0f);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,M,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Float(System.Action<float> M,float SV,float EV,float Time,float RewindTime)//Rewind.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setFloat(SV,EV,0f,0f);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,M,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Float(System.Action<float> M,float SV,float EV,float Time,bool UseSpeed)//Usespeed
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs((float)SV - (float)EV)/Time;}

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setFloat(SV,EV,0f,0f);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,M,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Float(System.Action<float> M,float SV,float EV,float Time,float FlickerMax,float FlickerMin)//Flicker.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Flicker,false);//Map the variable data.
			TempLerpScript.setFloat(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,M,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Float(System.Action<float> M,float SV,float EV,float Time)//This is the absolute minimum.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setFloat(SV,EV,0f,0f);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,M,null,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Double(System.Action<double> M,double SV,double EV,float Time,bool UseSpeed,double FlickerMax,double FlickerMin,LerpMode Mode,TypeOfLerp LerpType,float RewindTime)//Everything.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs((float)SV - (float)EV)/Time;}

			if(Mode != LerpMode.Flicker)
			{
				FlickerMax = 0;
				FlickerMin = 0;
			}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,LerpType,Mode,true);//Map the variable data.
			TempLerpScript.setDouble(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,M,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Double(System.Action<double> M,double SV,double EV,float Time,float RewindTime,bool UseSpeed)//Rewind and usespeed.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs((float)SV - (float)EV)/Time;}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setDouble(SV,EV,0,0);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,M,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Double(System.Action<double> M,double SV,double EV,float Time,float RewindTime)//Rewind.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setDouble(SV,EV,0,0);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,M,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Double(System.Action<double> M,double SV,double EV,float Time,bool UseSpeed)//Usespeed
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs((float)SV - (float)EV)/Time;}

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setDouble(SV,EV,0,0);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,M,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Double(System.Action<double> M,double SV,double EV,float Time,double FlickerMax,double FlickerMin)//Flicker.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Flicker,false);//Map the variable data.
			TempLerpScript.setDouble(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,M,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Double(System.Action<double> M,double SV,double EV,float Time)//This is the absolute minimum.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setDouble(SV,EV,0,0);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,M,null,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector2(System.Action<Vector2> M,Vector2 SV,Vector2 EV,float Time,bool UseSpeed,Vector2 FlickerMax,Vector2 FlickerMin,LerpMode Mode,TypeOfLerp LerpType,float RewindTime)//Everything.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.magnitude - EV.magnitude)/Time;}

			if(Mode != LerpMode.Flicker)
			{
				FlickerMax = UnityEngine.Vector2.zero;
				FlickerMin = UnityEngine.Vector2.zero;
			}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,LerpType,Mode,true);//Map the variable data.
			TempLerpScript.setV2(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,M,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector2(System.Action<Vector2> M,Vector2 SV,Vector2 EV,float Time,float RewindTime,bool UseSpeed)//Rewind and usespeed.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.magnitude - EV.magnitude)/Time;}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setV2(SV,EV,UnityEngine.Vector2.zero,UnityEngine.Vector2.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,M,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector2(System.Action<Vector2> M,Vector2 SV,Vector2 EV,float Time,float RewindTime)//Rewind.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setV2(SV,EV,UnityEngine.Vector2.zero,UnityEngine.Vector2.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,M,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector2(System.Action<Vector2> M,Vector2 SV,Vector2 EV,float Time,bool UseSpeed)//Usespeed
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.magnitude - EV.magnitude)/Time;}

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setV2(SV,EV,UnityEngine.Vector2.zero,UnityEngine.Vector2.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,M,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector2(System.Action<Vector2> M,Vector2 SV,Vector2 EV,float Time,Vector2 FlickerMax,Vector2 FlickerMin)//Flicker.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Flicker,false);//Map the variable data.
			TempLerpScript.setV2(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,M,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector2(System.Action<Vector2> M,Vector2 SV,Vector2 EV,float Time)//This is the absolute minimum.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setV2(SV,EV,UnityEngine.Vector2.zero,UnityEngine.Vector2.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,M,null,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector3(System.Action<Vector3> M,Vector3 SV,Vector3 EV,float Time,bool UseSpeed,Vector3 FlickerMax,Vector3 FlickerMin,LerpMode Mode,TypeOfLerp LerpType,float RewindTime)//Everything.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.magnitude - EV.magnitude)/Time;}

			if(Mode != LerpMode.Flicker)
			{
				FlickerMax = UnityEngine.Vector3.zero;
				FlickerMin = UnityEngine.Vector3.zero;
			}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,LerpType,Mode,true);//Map the variable data.
			TempLerpScript.setV3(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,M,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector3(System.Action<Vector3> M,Vector3 SV,Vector3 EV,float Time,float RewindTime,bool UseSpeed)//Rewind and usespeed.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.magnitude - EV.magnitude)/Time;}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setV3(SV,EV,UnityEngine.Vector3.zero,UnityEngine.Vector3.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,M,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector3(System.Action<Vector3> M,Vector3 SV,Vector3 EV,float Time,float RewindTime)//Rewind.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setV3(SV,EV,UnityEngine.Vector3.zero,UnityEngine.Vector3.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,M,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector3(System.Action<Vector3> M,Vector3 SV,Vector3 EV,float Time,bool UseSpeed)//Usespeed
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.magnitude - EV.magnitude)/Time;}

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setV3(SV,EV,UnityEngine.Vector3.zero,UnityEngine.Vector3.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,M,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector3(System.Action<Vector3> M,Vector3 SV,Vector3 EV,float Time,Vector3 FlickerMax,Vector3 FlickerMin)//Flicker.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Flicker,false);//Map the variable data.
			TempLerpScript.setV3(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,M,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector3(System.Action<Vector3> M,Vector3 SV,Vector3 EV,float Time)//This is the absolute minimum.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setV3(SV,EV,UnityEngine.Vector3.zero,UnityEngine.Vector3.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,M,null,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector4(System.Action<Vector4> M,Vector4 SV,Vector4 EV,float Time,bool UseSpeed,Vector4 FlickerMax,Vector4 FlickerMin,LerpMode Mode,TypeOfLerp LerpType,float RewindTime)//Everything.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.magnitude - EV.magnitude)/Time;}

			if(Mode != LerpMode.Flicker)
			{
				FlickerMax = UnityEngine.Vector4.zero;
				FlickerMin = UnityEngine.Vector4.zero;
			}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,LerpType,Mode,true);//Map the variable data.
			TempLerpScript.setV4(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,M,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector4(System.Action<Vector4> M,Vector4 SV,Vector4 EV,float Time,float RewindTime,bool UseSpeed)//Rewind and usespeed.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.magnitude - EV.magnitude)/Time;}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setV4(SV,EV,UnityEngine.Vector4.zero,UnityEngine.Vector4.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,M,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector4(System.Action<Vector4> M,Vector4 SV,Vector4 EV,float Time,float RewindTime)//Rewind.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setV4(SV,EV,UnityEngine.Vector4.zero,UnityEngine.Vector4.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,M,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector4(System.Action<Vector4> M,Vector4 SV,Vector4 EV,float Time,bool UseSpeed)//Usespeed
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.magnitude - EV.magnitude)/Time;}

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setV4(SV,EV,UnityEngine.Vector4.zero,UnityEngine.Vector4.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,M,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector4(System.Action<Vector4> M,Vector4 SV,Vector4 EV,float Time,Vector4 FlickerMax,Vector4 FlickerMin)//Flicker.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Flicker,false);//Map the variable data.
			TempLerpScript.setV4(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,M,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Vector4(System.Action<Vector4> M,Vector4 SV,Vector4 EV,float Time)//This is the absolute minimum.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setV4(SV,EV,UnityEngine.Vector4.zero,UnityEngine.Vector4.zero);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,M,null,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Colour(System.Action<Color> M,Color SV,Color EV,float Time,bool UseSpeed,Color FlickerMax,Color FlickerMin,LerpMode Mode,TypeOfLerp LerpType,float RewindTime)//Everything.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.grayscale - EV.grayscale)/Time;}

			if(Mode != LerpMode.Flicker)
			{
				FlickerMax = UnityEngine.Color.clear;
				FlickerMin = UnityEngine.Color.clear;
			}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,LerpType,Mode,true);//Map the variable data.
			TempLerpScript.setC(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,M,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Colour(System.Action<Color> M,Color SV,Color EV,float Time,float RewindTime,bool UseSpeed)//Rewind and usespeed.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.grayscale - EV.grayscale)/Time;}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setC(SV,EV,UnityEngine.Color.clear,UnityEngine.Color.clear);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,M,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Colour(System.Action<Color> M,Color SV,Color EV,float Time,float RewindTime)//Rewind.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);//Map the variable data.
			TempLerpScript.setC(SV,EV,UnityEngine.Color.clear,UnityEngine.Color.clear);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,M,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Colour(System.Action<Color> M,Color SV,Color EV,float Time,bool UseSpeed)//Usespeed
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			if(UseSpeed == true){Time = Mathf.Abs(SV.grayscale - EV.grayscale)/Time;}

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setC(SV,EV,UnityEngine.Color.clear,UnityEngine.Color.clear);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,M,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Colour(System.Action<Color> M,Color SV,Color EV,float Time,Color FlickerMax,Color FlickerMin)//Flicker.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Flicker,false);//Map the variable data.
			TempLerpScript.setC(SV,EV,FlickerMax,FlickerMin);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,M,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Colour(System.Action<Color> M,Color SV,Color EV,float Time)//This is the absolute minimum.
		{
			TempLerpScript = GetRefScript();//Get the lerp script.

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);//Map the variable data.
			TempLerpScript.setC(SV,EV,UnityEngine.Color.clear,UnityEngine.Color.clear);//Map the variable data.

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,M,null));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\





//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Quaternion(System.Action<Quaternion> M,Quaternion SV,Quaternion EV,float Time,bool UseSpeed,LerpMode Mode,TypeOfLerp LerpType,float RewindTime)//Everything.
		{
			TempLerpScript = GetRefScript();
			if(UseSpeed == true){Time = Mathf.Abs(SV.eulerAngles.magnitude - EV.eulerAngles.magnitude)/Time;}

			if(Mode == LerpMode.Flicker){Mode = LerpMode.Normal;}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,LerpType,Mode,true);
			TempLerpScript.setQ(SV,EV);

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,null,M));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Quaternion(System.Action<Quaternion> M,Quaternion SV,Quaternion EV,float Time,bool UseSpeed,float RewindTime)//Rewind and usespeed.
		{
			TempLerpScript = GetRefScript();
			if(UseSpeed == true){Time = Mathf.Abs(SV.eulerAngles.magnitude - EV.eulerAngles.magnitude)/Time;}

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);
			TempLerpScript.setQ(SV,EV);

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,null,M));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Quaternion(System.Action<Quaternion> M,Quaternion SV,Quaternion EV,float Time,float RewindTime)//Rewind.
		{
			TempLerpScript = GetRefScript();

			TempLerpScript.SetOtherNumbers(Time,RewindTime,TypeOfLerp.Lerp,LerpMode.Rewind,false);
			TempLerpScript.setQ(SV,EV);

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,null,M));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Quaternion(System.Action<Quaternion> M,Quaternion SV,Quaternion EV,float Time,bool UseSpeed)//Usespeed
		{
			TempLerpScript = GetRefScript();
			if(UseSpeed == true){Time = Mathf.Abs(SV.eulerAngles.magnitude - EV.eulerAngles.magnitude)/Time;}

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);
			TempLerpScript.setQ(SV,EV);

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,null,M));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
		public void Quaternion(System.Action<Quaternion> M,Quaternion SV,Quaternion EV,float Time)//This is the absolute minimum.
		{
			TempLerpScript = GetRefScript();

			TempLerpScript.SetOtherNumbers(Time,0f,TypeOfLerp.Lerp,LerpMode.Normal,false);
			TempLerpScript.setQ(SV,EV);

			StartCoroutine(TempLerpScript.Lerp(null,null,null,null,null,null,null,M));//Map and start the lerp.
		}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\





		LerpValue GetRefScript()//Fetch a script for the user.
		{
			LerpValue TempLerp = null;//If this is not set to null unity3d throws an error.

			if(InactiveScripts.Count > 0)//Is there an inactive lerp script on the books, so to speak?
			{
				TempLerp = InactiveScripts[0].gameObject.GetComponent<LerpValue>();//Set script reference.
				InactiveScripts[0].gameObject.transform.SetParent(ActiveParent);//Set the parent transform for tidiness.
				ActiveScripts.Add(InactiveScripts[0]);//Add to active list.
				InactiveScripts.Remove(InactiveScripts[0]);//Remove from inactive list.
			}
			else
			{
				if(RefLerpTemplate != null)//Check that we have a prefab assigned to actually spawn.
				{
					GameObject TempObject = Instantiate(RefLerpTemplate) as GameObject;//Generate the new script.
					TempLerp = TempObject.GetComponent<LerpValue>();//Set script reference.
					TempObject.transform.SetParent(ActiveParent);//Set the parent transform for tidiness.
					ActiveScripts.Add(TempObject);//Add to active list.
				}
				else
				{
					Debug.Log("The ReferencePrefab is set to null so the lerp has failed to start");//Send error message.
				}
			}

			if(TempLerp != null)
			{
				TempLerp.Manager = this;//Set scripts lerpManager reference to this script.
			}
			else
			{
				print("asdfasdf");
			}
			return TempLerp;//Return the result.
		}







		LerpReference GetRefLerpScript()//Fetch a script for the user.
		{
			LerpReference TempLerp = null;//If this is not set to null unity3d throws an error.

			if(InactiveRefScripts.Count > 0)//Is there an inactive lerp script on the books, so to speak?
			{
				TempLerp = InactiveRefScripts[0].gameObject.GetComponent<LerpReference>();//Set script reference.
				InactiveRefScripts[0].gameObject.transform.SetParent(ActiveParent);//Set the parent transform for tidiness.
				ActiveRefScripts.Add(InactiveRefScripts[0]);//Add to active list.
				InactiveRefScripts.Remove(InactiveRefScripts[0]);//Remove from inactive list.
			}
			else
			{
				if(RefLerpRefTemplate != null)//Check that we have a prefab assigned to actually spawn.
				{
					GameObject TempObject = Instantiate(RefLerpRefTemplate) as GameObject;//Generate the new script.
					TempLerp = TempObject.GetComponent<LerpReference>();//Set script reference.
					TempObject.transform.SetParent(ActiveParent);//Set the parent transform for tidiness.
					ActiveRefScripts.Add(TempObject);//Add to active list.
				}
				else
				{
					Debug.Log("The ReferencePrefab is set to null so the lerp has failed to start");//Send error message.
				}
			}

			if(TempLerp != null)
			{
				TempLerp.Manager = this;//Set scripts lerpManager reference to this script.
			}
			else
			{
				print("asdfasdf");
			}
			return TempLerp;//Return the result.
		}
















		public void LerpRefTransforms(Transform ObjectToMove,Transform[] Temp,float MoveTime,bool useSpeed,TypeOfLerp T,bool Repeat)
		{
			TempLerpRefScript = GetRefLerpScript();

			TempLerpRefScript.MapTransforms(ObjectToMove,Temp,MoveTime,useSpeed,T,Repeat);
		}

		public void LerpRefTransform(Transform ObjectToMove,Transform Temp,float MoveTime,bool useSpeed,TypeOfLerp T,bool Repeat,bool EndAtStart)
		{
			TempLerpRefScript = GetRefLerpScript();

			Transform TempStart = new GameObject().transform;

			TempStart.position = ObjectToMove.position;
			TempStart.rotation = ObjectToMove.rotation;

			if(EndAtStart == true)
			{
				TempLerpRefScript.MapTransforms(ObjectToMove,new Transform[3]{TempStart,Temp,TempStart},MoveTime,useSpeed,T,Repeat);
			}
			else
			{
				TempLerpRefScript.MapTransforms(ObjectToMove,new Transform[2]{TempStart,Temp},MoveTime,useSpeed,T,Repeat);
			}
		}			
	}
}