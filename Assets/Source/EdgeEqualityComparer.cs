using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeEqualityComparer : IEqualityComparer<Edge> {

    public bool Equals(Edge a, Edge b)
    {
        if (a.vertexA == b.vertexA && a.vertexB == b.vertexB)
        {
            return true;
        }

        return false;
    }

    public int GetHashCode(Edge edge)
    {
        int hCode = 58392873;
        hCode = hCode * -2967163 + edge.vertexA.GetHashCode();
        hCode = hCode * -2967163 + edge.vertexB.GetHashCode();
        return hCode.GetHashCode();
    }
}
