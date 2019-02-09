using UnityEngine;

public struct Trans
{
    public Trans(Transform T)
    {
        position = T.position;
        rotation = T.rotation;
        scale = T.localScale;
    }
    
    /// <summary>
    /// World position
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// World rotation
    /// </summary>
    public Quaternion rotation;

    /// <summary>
    /// Local scale
    /// </summary>
    public Vector3 scale;
}
