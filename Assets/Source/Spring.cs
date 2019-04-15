using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spring script
/// </summary>
public class Spring
{
    /// <summary>
    /// Default constructor. Zero all. 
    /// </summary>
    public Spring(Node nodeA, Node nodeB, float stiffness, float beta)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
        length0 = Vector3.Distance(nodeA.pos, nodeB.pos);
        this.stiffness = stiffness;
        this.beta = beta;
    }


    #region InEditorVariables

    public Node nodeA;
    public Node nodeB;

    public float stiffness;
    public float beta;


    #endregion

    #region OtherVariables

    public float length0;


    #endregion

    #region OtherFunctions

    public float getLength()
    {
        return Vector3.Distance(nodeA.pos, nodeB.pos);
    }

    public void computeForce()
    {
        Vector3 force = - stiffness * (getLength() - length0) * getUnitVector();
        force -= (beta * stiffness) * Vector3.Dot(getUnitVector(), (nodeA.vel - nodeB.vel)) * getUnitVector();
        nodeA.force += force;
        nodeB.force -= force;
    }

    public Vector3 getUnitVector()
    {
        return (nodeA.pos - nodeB.pos) / getLength();
    }

    #endregion



}
