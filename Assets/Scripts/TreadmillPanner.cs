using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TreadmillPanner : MonoBehaviour {

    public static readonly Vector3 GIZMO_SIZE = new Vector3(5.0f, 0.1f, 200.0f);

    //Blueprint Prefabs to use as base for the treadmill
    public MeshRenderer[] GroundBlueprints;

    //Number of sections to render
    public int NumSections = 5;

    //Velocity of the treadmill
    public float Velocity = 20;

    //A circular list that maintains references to each section
    private MeshRenderer[] _circularList;

    //Points to the last generated treadmill section
    private int _currentIndex;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5F);

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        //Gizmos.matrix = rotationMatrix;

        Vector3 origin = transform.position + transform.forward * GIZMO_SIZE.z / 2.0f;
        Debug.Log(origin);
        Gizmos.DrawCube(origin, GIZMO_SIZE);
        Gizmos.DrawWireCube(origin, GIZMO_SIZE);
    }
#endif

    void Start()
    {
        if(GroundBlueprints.Length == 0)
        {
            Debug.LogError("No ground blueprints found, cannot continue with the treadmill");
            return;
        }

        //First of all, create the circular list
        _circularList = new MeshRenderer[NumSections];

        for(int i = 0; i < NumSections; i++)
        {
            MeshRenderer renderer = GameObject.Instantiate<MeshRenderer>(GroundBlueprints[Random.Range(0, GroundBlueprints.Length)]);//@TODO: maybe use an object pool for the ground?
            renderer.transform.parent = transform;
            renderer.transform.rotation = transform.rotation;
            renderer.transform.position = transform.position;

            _circularList[i] = renderer;
        }

        _currentIndex = 0;

        //Should give the pieces a first sort
        SortPieces();
    }

    void SortPieces()
    {
        //All pieces follow the current piece position
        float forwardLength = _circularList[_currentIndex].transform.localPosition.z;
        for(int i = 0; i < NumSections; i++)
        {
            int circularIndex = (_currentIndex + i) % NumSections;
            MeshRenderer renderer = _circularList[circularIndex];
            renderer.transform.position = transform.position + transform.forward * forwardLength;

            //Add the length
            Debug.Log("Bounds: " +renderer.bounds.size);
            forwardLength += renderer.bounds.size.z;
        }

    }

    // Update is called once per frame
    void Update () {
        //We only need to update the position of the current index
        MeshRenderer renderer = _circularList[_currentIndex];
        renderer.transform.localPosition += Vector3.forward * Velocity * Time.deltaTime;

        //If we passed the point of swapping (that is, the origin). Swap
        if(renderer.transform.localPosition.z > 0)
        {
            //Grab the previous point on the circular index
            int previousIndex = _currentIndex - 1;
            previousIndex += previousIndex < 0 ? NumSections : 0; //do a loop

            MeshRenderer lastRenderer = _circularList[previousIndex];
            lastRenderer.transform.position = renderer.transform.position - transform.forward * lastRenderer.bounds.size.z;
            _currentIndex = previousIndex;
        }

        //Sort the pieces
        SortPieces();
	}
}
