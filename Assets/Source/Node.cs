using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///Node should have a PhysicsManager as a parent for proper use

/// <summary>
/// Node script
/// </summary>
public class Node
{
    /// <summary>
    /// Default constructor. Zero all. 
    /// </summary>
    public Node(Vector3 pos, Vector3 gravity, float mass, float alpha)
    {
        this.pos = pos;
        this.vel = Vector3.zero;
        this.mass = mass;
        this.gravity = gravity;
        this.alpha = alpha;
    }


    #region InEditorVariables

    public float mass;
    public Vector3 pos;
    public Vector3 vel;
    public Vector3 force;
    public bool isFixed;
    public Vector3 gravity;
    public float alpha;
    public float area;

    #endregion
    #region OtherVariables
    #endregion
 

    #region OtherFunctions

    public void computeForce()
    {
        force += mass * gravity;
        force -= alpha * mass * vel;
    }

    #endregion

}
