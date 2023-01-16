using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

public class CubeMaze : MonoBehaviour
{

    public ushort size;
    public GameObject[] segments;
    public Transform buildPreview;
    public Material buildPreviewOpen;
    public Material buildPreviewBlocked;

    [HideInInspector]
    public ushort layer;
    [HideInInspector]
    public CubeMazeSegment selectedSegment;
    public Node[,,] nodes;

    void OnValidate()
    {
        Debug.Log("Subscribe Selection Changed");
        Selection.selectionChanged -= OnSelectionChanged;
        Selection.selectionChanged += OnSelectionChanged;
    }

    void Start()
    {
        Application.targetFrameRate = 120;
    }

    void OnSelectionChanged()
    {
        try
        {
            if (Selection.activeObject == this.gameObject)
                ToolManager.SetActiveTool<CubeMazeEditorTool>();
        } catch {}
    }

    public void Initialize()
    {
        nodes = new Node[size, size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    var node = new Node();
                    if (x > 0) node.edges[0] = nodes[x - 1, y, z].edges[3] = new Edge(node, nodes[x - 1, y, z]);
                    if (y > 0) node.edges[1] = nodes[x, y - 1, z].edges[4] = new Edge(node, nodes[x, y - 1, z]);
                    if (z > 0) node.edges[2] = nodes[x, y, z - 1].edges[5] = new Edge(node, nodes[x, y, z - 1]);
                    nodes[x, y, z] = node;
                }
            }
        }
    }
}

public enum CubeMazeSegment
{
    Straight = 0, End = 1, CurveSmall = 2, CurveBig = 3, Wall = 4, FinishTrigger = 5
}
public enum NodeState
{
    open, blocked
}
public enum EdgeState
{
    open, blocked, openConnection, blockedConnection
}
public class Node
{
    public NodeState state = NodeState.open;
    public Edge[] edges = new Edge[6];
}
public class Edge
{
    public EdgeState state = EdgeState.open;
    public Node[] nodes;
    public Edge(Node a, Node b)
    {
        nodes = new Node[2] { a, b };
    }
}
