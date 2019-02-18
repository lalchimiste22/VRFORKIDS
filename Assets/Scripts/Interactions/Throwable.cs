using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Draggable))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class Throwable : MonoBehaviour
{
    //Cached required variables
    private Rigidbody _rb;
    private Draggable _draggable;
    private LineRenderer _lr;
    private float g;

    /// <summary>
    /// Impulse force used when throwing
    /// </summary>
    public float Impulse = 5.0f;

    /// <summary>
    /// Amount of time, in seconds, that the script waits before throwing the object after being dragged.
    /// </summary>
    public float Delay = 5.0f;

    /// <summary>
    /// Amount of time the Drag operation will be deactivated after being thrown
    /// </summary>
    public float PostThrowDragTime = 3.0f;

    /// <summary>
    /// Number of line segments to show for the arc render.
    /// </summary>
    public int RenderResolution = 10;

    /// <summary>
    /// Length of each segments for the arc render
    /// </summary>
    public float SegmentSize = 0.25f;

    //Control variables
    private bool bDragging = false;
    private float RemainingTime;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _draggable = GetComponent<Draggable>();
        _lr = GetComponent<LineRenderer>();

        g = Mathf.Abs(Physics.gravity.y);

        if(_draggable)
        {
            _draggable.OnDragStart.AddListener(OnDragStart);
            _draggable.OnDragEnd.AddListener(OnDragEnd);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(bDragging)
        {
            RemainingTime -= Time.deltaTime;

            if(RemainingTime <= 0.0f)
            {
                //Should apply force to the rigidbody, first we need to tell the draggable component to stop.
                _draggable.CommitDragOperation();
                StartCoroutine(DragTimeDeactivator());

                //Apply force on the direction we're looking
                _rb.AddForce(Camera.main.transform.forward * Impulse, ForceMode.Impulse);
            }
            else
            {
                //We're dragging but not yet ready to throw, show some UI
                RenderArc();
            }
        }
        else
        {
            //Disable line renderer
            _lr.enabled = false;
        }
    }

    void OnDragEnd()
    {
        bDragging = false;
    }

    void OnDragStart()
    {
        RemainingTime = Delay;
        bDragging = true;
    }

    IEnumerator DragTimeDeactivator()
    {
        _draggable.IsDraggable = false;
        yield return new WaitForSeconds(PostThrowDragTime);
        _draggable.IsDraggable = true;
    }

    void RenderArc()
    {
        _lr.enabled = true;
        _lr.positionCount = RenderResolution + 1;
        _lr.SetPositions(GetArcPoints());
    }

    Vector3[] GetArcPoints()
    {
        Vector3[] Points = new Vector3[RenderResolution + 1];
        float RadAngle = Mathf.Deg2Rad * Camera.main.transform.rotation.eulerAngles.x;
        float Velocity = Impulse / _rb.mass;
        Vector3 OffsetFromCamera = transform.position - Camera.main.transform.position;

        for (int i = 0; i <= RenderResolution; i++)
        {
            //Calculate on local space
            float x = i * SegmentSize;
            float y = x * Mathf.Tan(RadAngle) - (g * x * x) / (2 * Mathf.Pow(Mathf.Cos(RadAngle) * Velocity, 2));

            Points[i] = Camera.main.transform.TransformPoint(new Vector3(0.0f, y, x)) + OffsetFromCamera;
        }

        return Points;
    }
}
