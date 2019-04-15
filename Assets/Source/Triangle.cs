using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle {

    public Triangle(Node nodeA, Node nodeB, Node nodeC)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
        this.nodeC = nodeC;
    }

    #region Variables

    Node nodeA;
    Node nodeB;
    Node nodeC;

    #endregion
    #region Functions

    public Vector3 getNormal()
    {
        return (Vector3.Cross((nodeB.pos - nodeA.pos), (nodeC.pos - nodeA.pos))).normalized;
    }

    public float getArea()
    {
        return Vector3.Cross((nodeB.pos - nodeA.pos), (nodeC.pos - nodeA.pos)).magnitude * 0.5f;
    }

    public Vector3 getVel()
    {
        return (nodeA.vel + nodeB.vel + nodeC.vel) / 3.0f;
    }

    public void computeForce(float friction, Vector3 windVel)
    {
        Vector3 force = friction * getArea() * Vector3.Dot(getNormal(), (windVel - getVel())) * getNormal();

        nodeA.force += force/3;
        nodeB.force += force/3;
        nodeC.force += force/3;
    }
    #endregion
}
