using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Basic physics manager capable of simulating a given ISimulable
/// implementation using diverse integration methods: explicit,
/// implicit, Verlet and semi-implicit.
/// </summary>
public class MassSpringCloth : MonoBehaviour 
{
	/// <summary>
	/// Default constructor. Zero all. 
	/// </summary>
	public MassSpringCloth()
	{
		this.Paused = true;
		this.TimeStep = 0.001f;
		this.Gravity = new Vector3 (0.0f, -9.81f, 0.0f);
		this.IntegrationMethod = Integration.Symplectic;
	}

	/// <summary>
	/// Integration method.
	/// </summary>
	public enum Integration
	{
		Explicit = 0,
		Symplectic = 1,
	};

	#region InEditorVariables

	public bool Paused;
	public float TimeStep;
    public Vector3 Gravity;
	public Integration IntegrationMethod;

    Mesh mesh;
    Vector3[] vertices;
    List<Spring> springs = new List<Spring>();
    List<Node> nodes = new List<Node>();
    List<Triangle> clothTriangles = new List<Triangle>();

    public float nodeMass = 0.5f;
    public float stiffnessFlex = 50.0f;
    public float stiffnessTrac = 500.0f;

    public float dampAlpha = 0.1f;
    public float dampBeta = 0.1f;

    public float friction = 1;
    public Vector3 windVel = Vector3.zero;

    #endregion

    #region OtherVariables

    #endregion


    #region Getters and Setters

    public List<Node> getNodes()
    {
        return nodes;
    }

    public void setNodes(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    #endregion

    #region MonoBehaviour

    public void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        foreach(Vector3 v in vertices)
        {
            nodes.Add(new Node(transform.TransformPoint(v), Gravity, nodeMass, dampAlpha));
        }
        EdgeEqualityComparer edgeComparer = new EdgeEqualityComparer();

        var edgeDictionary = new Dictionary<Edge, Edge>(edgeComparer);

        for (int i = 0; i < triangles.Length;i += 3)
        {
            //Arista 1
            Edge edge = new Edge(triangles[i], triangles[i + 1], triangles[i + 2]);
            tryAddEdge(edge, edgeDictionary);
            //Arista 2
            edge = new Edge(triangles[i + 1], triangles[i + 2], triangles[i]);
            tryAddEdge(edge, edgeDictionary);
            //Arista 3
            edge = new Edge(triangles[i], triangles[i + 2], triangles[i + 1]);
            tryAddEdge(edge, edgeDictionary);

            //Añade triangulo a la lista
            clothTriangles.Add(new Triangle(nodes[triangles[i]], nodes[triangles[i + 1]], nodes[triangles[i + 2]]));
        } 
        foreach(KeyValuePair<Edge, Edge> e in edgeDictionary)
        {
            springs.Add(new Spring(nodes[e.Key.vertexA], nodes[e.Key.vertexB], stiffnessTrac, dampBeta));
        }
    }

	public void Update()
	{
		if (Input.GetKeyUp (KeyCode.P))
			this.Paused = !this.Paused;

        for(int i = 0; i < nodes.Count; i++)
        {
            vertices[i] = transform.InverseTransformPoint(nodes[i].pos);
        }
        mesh.vertices = vertices;
	}

    public void FixedUpdate()
    {
        for(int i = 0; i < 10; i++)
        {
            if (this.Paused)
                return; // Not simulating

            // Select integration method
            switch (this.IntegrationMethod)
            {
                case Integration.Explicit: this.stepExplicit(); break;
                case Integration.Symplectic: this.stepSymplectic(); break;
                default:
                    throw new System.Exception("[ERROR] Should never happen!");
            }
        }

    }

    #endregion


    private void tryAddEdge(Edge edge, Dictionary<Edge, Edge> dictionary)
    {
        Edge otherEdge;
        if(dictionary.TryGetValue(edge, out otherEdge))
        {
            springs.Add(new Spring(nodes[edge.vertexC], nodes[otherEdge.vertexC], stiffnessFlex, dampBeta));

        }
        else
        {
            dictionary.Add(edge, edge);
        }
    }
    /// <summary>
    /// Performs a simulation step in 1D using Explicit integration.
    /// </summary>
    private void stepExplicit()
	{
        //Unstable, requires lower timestep
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].force = Vector3.zero;
            nodes[i].computeForce();
        }
        for (int i = 0; i < springs.Count; i++)
        {
            springs[i].computeForce();
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isFixed)
            {
                nodes[i].pos += TimeStep * nodes[i].vel;
                nodes[i].vel += (TimeStep / nodes[i].mass) * nodes[i].force;
            }
        }
    }

	/// <summary>
	/// Performs a simulation step in 1D using Symplectic integration.
	/// </summary>
	private void stepSymplectic()
	{
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].force = Vector3.zero;
            nodes[i].computeForce();
        }
        for (int i = 0; i < springs.Count; i++)
        {
            springs[i].computeForce();
        }
        for (int i = 0; i < clothTriangles.Count; i++)
        {
            clothTriangles[i].computeForce(friction, windVel);
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isFixed)
            {
                nodes[i].vel += (TimeStep / nodes[i].mass) * nodes[i].force;
                nodes[i].pos += TimeStep * nodes[i].vel;
            }
        }

    }
}
