using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZoomInDetail : MonoBehaviour {

    public UnityEvent OnShown;
    public UnityEvent OnExit; //Not proud of naming here, should change

    private static float GIZMOS_RADIUS = 0.15f;

    /// <summary>
    /// Player to move
    /// </summary>
    private EaseMoveTo _playerObject;

    /// <summary>
    /// The position to move to
    /// </summary>
    public Transform TargetPosition;

    /// <summary>
    /// The transform to go back to
    /// </summary>
    public Transform ExitTransform;

    /// <summary>
    /// If the zoom detail should force the rotation of the player
    /// </summary>
    public bool ShouldForceRotation = true;

    /// <summary>
    /// The canvas or overall gameobject that will be SetActive when entering/exiting the zoom detail
    /// </summary>
    public GameObject ZoomCanvas;

    /// <summary>
    /// Exit point
    /// </summary>
    private Trans ExitPoint;

    /// <summary>
    /// Needs to initialize
    /// </summary>
    private bool bPendingInitialization = true;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, GIZMOS_RADIUS);

        if (TargetPosition)
        {
            Gizmos.DrawLine(transform.position, TargetPosition.position);
            Gizmos.color = new Color(1.0f, 0.0f, 1.0f);

            Gizmos.DrawSphere(TargetPosition.position, GIZMOS_RADIUS);
        }
    }
#endif

    public void Initialize()
    {
        _playerObject = MyManager.Instance.playerGameObject.GetComponent<EaseMoveTo>();

        if (!_playerObject)
            Debug.LogError("No player object with EaseMovment found");

        bPendingInitialization = false;
    }

    public void Start()
    {
        Initialize();
    }

    public void ShowZoomDetail()
    {
        if(bPendingInitialization)
            Initialize();

        if (ExitTransform == null)
            ExitPoint = new Trans(MyManager.Instance.playerGameObject.transform);
        else
            ExitPoint = new Trans(ExitTransform);

        _playerObject.SmoothTransport(new Trans(TargetPosition),-1,null,!ShouldForceRotation);

        if (ZoomCanvas)
            ZoomCanvas.SetActive(true);

        if (OnShown != null)
            OnShown.Invoke();
    }

    public void ExitZoomDetail()
    {
        if (bPendingInitialization)
            Initialize();

        _playerObject.SmoothTransport(ExitPoint, -1, null, !ShouldForceRotation);

        if (ZoomCanvas)
            ZoomCanvas.SetActive(false);

        if (OnExit != null)
            OnExit.Invoke();
    }
}
