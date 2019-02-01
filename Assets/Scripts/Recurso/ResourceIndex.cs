using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Holds a resource registry index for knowing in which stage to get an specific resource by its code.
/// Can be baked, which will sweep every BuildStage and queue their existent resources
/// </summary>
public class ResourceIndex : MonoBehaviour
{
    /// <summary>
    /// Map holding the index of this object, in the form of {Code, StagePath}
    /// </summary>
    private Dictionary<string, string> IndexMap;

    /// <summary>
    /// Object that is responsible of baking the resources, as it can change scenes and don't get deleted
    /// </summary>
    private static ResourceBaker Baker = new ResourceBaker();

    //@NOTE: just for testing UNDO history
    public int test = 0;

    /// <summary>
    /// Start the baking process
    /// </summary>
    public void Bake()
    {
        Baker.BeginBake(gameObject);
    }
    
    /// <summary>
    /// Called when the baking is completed
    /// </summary>
    /// <param name="IndexMap">New index map to use</param>
    public void OnCommitBake(Dictionary<string, string> NewIndexMap)
    {
        Undo.RecordObject(this, "Bake resource index");
        IndexMap = NewIndexMap;
        test++;
    }

    /// <summary>
    /// Internal class specialized on building the bake index for the resources.
    /// </summary>
    private class ResourceBaker : UnityEngine.Object
    {
        struct ResourceBakeContext
        {
            public ResourceBakeContext(string Name, string Scene)
            {
                CallerName = Name;
                CallerScene = Scene;
            }

            /// <summary>
            /// Name that we can use to get a reference of the caller, once we finish baking
            /// </summary>
            public string CallerName { get; private set; }

            /// <summary>
            /// Scene from which the baking was inited
            /// </summary>
            public string CallerScene { get; private set; }
        }

        /// <summary>
        /// Current index map being baked
        /// </summary>
        private Dictionary<string, string> IndexMap;

        /// <summary>
        /// Build scenes that we're currently baking
        /// </summary>
        private UnityEditor.EditorBuildSettingsScene[] BuildScenes;

        /// <summary>
        /// Current scene we're baking
        /// @see BuildScenes
        /// </summary>
        private int CurrentBakeScene;

        /// <summary>
        /// Holds the context in which the bake was requested
        /// </summary>
        private ResourceBakeContext BakeContext;

        public void BeginBake(GameObject Context)
        {
            //Make sure we have a clean bake
            IndexMap = new Dictionary<string, string>();
            BuildScenes = UnityEditor.EditorBuildSettings.scenes;
            CurrentBakeScene = 0;

            //Store context
            BakeContext = new ResourceBakeContext(Context.name, UnityEngine.SceneManagement.SceneManager.GetActiveScene().path);

            if (BuildScenes.Length > 0)
            {
                //Will need to manage level loading for resources
                UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnBakeLevelOpened;

                //Begin baking, we will start with the current bake scene
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(BuildScenes[CurrentBakeScene].path);
            }
        }

        private void EndBake()
        {
            //This isn't strictly needed, but it helps a lot with readability
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= OnBakeLevelOpened;
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnFinishBakeLevelOpened;

            //Finally open the context scene
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(BakeContext.CallerScene);
        }

        /// <summary>
        /// Called when loading a scene while on the baking process
        /// </summary>
        /// <param name="scene">Scene that was loaded</param>
        /// <param name="mode"> Mode in which the scene was loaded </param>
        void OnBakeLevelOpened(Scene scene, OpenSceneMode mode)
        {
            //Resources will most likely be set as inactive, so we need to obtain them using the scene manager
            GameObject[] Roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (GameObject go in Roots)
            {
                List<Recurso> Recursos = new List<Recurso>();
                go.GetComponentsInChildren<Recurso>(true, Recursos);

                //Need to add to the baked list
                foreach (Recurso r in Recursos)
                {
                    IndexMap.Add(r.Codigo, scene.path);
                }
            }

            //Finished baking of this scene, request the next one
            if (BuildScenes.Length > ++CurrentBakeScene)
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(BuildScenes[CurrentBakeScene].path);
            }
            else
            {
                EndBake();
            }
        }

        /// <summary>
        /// Called when loading the finishing bake level, separated from previous method for clarity
        /// </summary>
        /// <param name="scene">Scene opened</param>
        /// <param name="mode">Mode in which the scene was opened</param>
        void OnFinishBakeLevelOpened(Scene scene, OpenSceneMode mode)
        {
            //No matter if we succeed or fail now, we don't need to react to scene changes until next bake
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= OnFinishBakeLevelOpened;

            GameObject Caller = GameObject.Find(BakeContext.CallerName);
            ResourceIndex Index = Caller != null ? Caller.GetComponentInChildren<ResourceIndex>() : null;
            if(Index)
            {
                Index.OnCommitBake(IndexMap);
                Debug.Log("Baking complete");
            }
            else
            {
                Debug.LogError("Baking failed, could not locate baking context caller when reloading scene.");
            }
        }
    }
}
