using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HotspotMovement : MonoBehaviour {

    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;

    private static float GIZMOS_RADIUS = 0.15f;

    /// <summary>
    /// Movement component
    /// </summary>
    private EaseMoveTo _playerObject;

    /// <summary>
    /// Position to use instead of this gameobject's transform
    /// </summary>
    public Transform PositionOverride;

    /// <summary>
    /// If this instance requires a flush of the registered ActiveHotspots (will trigger OnDeactivated messages)
    /// </summary>
    public bool RequiresFlush = true;

    /// <summary>
    /// Current hotspot enabled, stored for sending messages internally
    /// </summary>
    private static List<HotspotMovement> ActiveHotspots;

    /// <summary>
    /// Used for lazy loading dependencies
    /// </summary>
    private bool bNeedsInitialize = true;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, GIZMOS_RADIUS);

        if(PositionOverride)
        {
            Gizmos.DrawLine(transform.position, PositionOverride.position);
            Gizmos.color = new Color(1.0f, 0.0f, 1.0f);

            Gizmos.DrawSphere(PositionOverride.position, GIZMOS_RADIUS);
        }
    }
#endif

    public static void Flush()
    {
        //We ask for deactivation without unregistering, so we do not modify the array inside the foreach
        foreach (HotspotMovement hotspot in ActiveHotspots)
            hotspot.Deactivate(false);

        ActiveHotspots.Clear();
    }

    public static void RegisterActiveHotspot(HotspotMovement hotspot)
    {
        ActiveHotspots.Add(hotspot);
    }

    public static void UnregisterActiveHotspot(HotspotMovement hotspot)
    {
        ActiveHotspots.Remove(hotspot);
    }

    public void Awake()
    {
        if (ActiveHotspots == null)
            ActiveHotspots = new List<HotspotMovement>();
    }

    private void Initialize()
    {
       _playerObject = MyManager.Instance.playerGameObject.GetComponent<EaseMoveTo>();

        if (!_playerObject)
            Debug.LogError("No player object with EaseMovment found");

        bNeedsInitialize = false;
    }

    /// <summary>
    /// Activates this hotspot, setting it as the currently active one. Sends OnDeactivate messages to any hotspot that was activated previously
    /// </summary>
    public void Activate()
    {
        if (bNeedsInitialize)
            Initialize();

        //Disable any hotspot currently running
        if (RequiresFlush && ActiveHotspots.Count > 0)
            HotspotMovement.Flush();

        //Move the character and register
        _playerObject.SmoothTransport(new Trans(PositionOverride ? PositionOverride : gameObject.transform), -1, null, true);
        HotspotMovement.RegisterActiveHotspot(this);

        //Call event dispatcher
        if(OnActivated != null)
            OnActivated.Invoke();
    }

    /// <summary>
    /// Deactivates this hotspot
    /// </summary>
    public void Deactivate(bool ShouldUnregister = true)
    {
        if (bNeedsInitialize)
            Initialize();

        //Remove from list
        if (ShouldUnregister)
            HotspotMovement.UnregisterActiveHotspot(this);

        //Call event dispatcher
        if(OnDeactivated != null)
            OnDeactivated.Invoke();
    }
}
