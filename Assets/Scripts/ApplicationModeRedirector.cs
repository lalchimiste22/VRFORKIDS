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
    /// Selection of the resource to focus is done on that level.<
    /// </summary>
    void PerformPreviewRedirection()
    {
        Debug.Log("Redirecting to preview mode scene");
    }
}
