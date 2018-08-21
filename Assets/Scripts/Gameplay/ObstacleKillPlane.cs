using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObstacleKillPlane : MonoBehaviour {

    //Tag that we will be looking for on a possible obstacle
    [TagSelector]
    public string Tag;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Detected Hit");

        PooledObject pooled = collision.collider.GetComponent<PooledObject>();

        if (pooled && pooled.tag == Tag)
            pooled.IsPooled = true;
    }
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Detected Hit");

        PooledObject pooled = collider.GetComponent<PooledObject>();

        if (pooled && pooled.tag == Tag)
            pooled.IsPooled = true;
    }
}
