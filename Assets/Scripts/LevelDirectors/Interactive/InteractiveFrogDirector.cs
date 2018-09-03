using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveFrogDirector : MonoBehaviour {

    //Geometry as enum, because they could add another layer
    public enum FrogGeometryType
    {
        Full,
        Skeleton
    };

    /// <summary>
    /// GameObject that wraps the full body of the frog
    /// </summary>
    public Animator TongueAnimator;

    /// <summary>
    /// Info point for the tongue
    /// </summary>
    public GameObject TongueInfoPoint;

    /// <summary>
    /// GameObject that wraps the full body of the frog
    /// </summary>
    public GameObject FullBody;

    /// <summary>
    /// GameObject that wraps the skeleton of the frog
    /// </summary>
    public GameObject SkeletonBody;

    /// <summary>
    /// Current geometry
    /// </summary>
    private FrogGeometryType _type;

    /// <summary>
    /// Current geometry
    /// </summary>
    private bool _tongueOutside;

    public void ToggleFrogGeometry()
    {
        switch(_type)
        {
            case FrogGeometryType.Full:
                _type = FrogGeometryType.Skeleton;
                FullBody.SetActive(false);
                SkeletonBody.SetActive(true);
                break;
            case FrogGeometryType.Skeleton:
                _type = FrogGeometryType.Full;
                FullBody.SetActive(true);
                SkeletonBody.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void ToggleTonguePosition()
    {
        _tongueOutside = !_tongueOutside;

        if(_tongueOutside)
            TongueAnimator.Play("frog tongue");
        else
            TongueAnimator.Play("frog tongue out");

        TongueInfoPoint.SetActive(_tongueOutside);
    }
}
