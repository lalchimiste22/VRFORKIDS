using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Redirects the game flow towards a specific scene when ApplicationMode.Preview is enabled by searching the Resource DB.
/// If normal mode is enabled, goes to the configured main scene.
/// </summary>
public class ApplicationModeRedirector : MonoBehaviour
{
    /// <summary>
    /// Name of the main scene to fall back when normal mode is enabled instead of preview mode.
    /// </summary>
    public string MainScenePath;

    /// <summary>
    /// Holds an indexed array for the Resource codes and its scenes.
    /// </summary>
    public ResourceIndex Index;

    // Start is called before the first frame update
    void Start()
    {
        //Redirection depends of the current application mode
        ApplicationMode Mode = MyManager.Instance.AppMode;
        switch(Mode)
        {
            case ApplicationMode.Normal:
                MyManager.Instance.loader.LoadScene(MainScenePath);
                break;
            case ApplicationMode.Preview:
                PerformPreviewRedirection();
                break;
        }
    }

    /// <summary>
    /// Redirects to the chosen scene for the preview mode.
    /// Selection of the resource to focus is done on that level.
    /// </summary>
    void PerformPreviewRedirection()
    {
        string RedirectPath;
        string ResourceCode = MyManager.Instance.PreviewResourceCode;
        if (Index.FindResourceScene(MyManager.Instance.PreviewResourceCode, out RedirectPath))
        {
            Debug.Log("Redirecting to preview mode scene");
            MyManager.Instance.loader.LoadScene(RedirectPath);
        }
        else
        {
            Debug.LogWarning("Trying to redirect to scene holding resource with code: '" + ResourceCode + "' failed, resource path not found on ResourceIndex");
        }
    }
}
