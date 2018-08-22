using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RunnerCharacterController : MonoBehaviour {


    //The game object to move, as if it where our character
    public Transform Pawn;

    //If the controller can actually do anything
    public bool HasControl = true;
    
    public abstract Collider GetControlledPawnCollider();
}
