using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dirty enum
public enum NODE_STATE { OPEN, CLOSED, ERROR, PATH };
public class Node : MonoBehaviour, IHeapItem<Node>
{
    public List<GameObject> ConnectedNodes = new List<GameObject>();

    [SerializeField]
    public Node ParentNode { get; set; }

    /// <summary>
    /// list of weighting for each connection in order.
    /// </summary>
    public List<float> ConnectionCosts = new List<float>();

    public int x, y;
    public bool walkable;
    public int weight;
    public int g;
    public int h;
    public float f { get { return g + h; } }

    public List<Node> GetNodeValues ()
    {
        List<Node> temp = new List<Node>();

        for (int i = 0; i < ConnectedNodes.Count; i++)
        {
            //temp.Add(new Node(ConnectedNodes[i].GetComponent<Node>(), ConnectionCosts[i], ConnectedNodes[i].GetComponent<Tile>().IsWalkable));
            temp.Add(ConnectedNodes[i].GetComponent<Node>());
        }

        return temp;
    }

    int heapIndex;
    
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public int CompareTo (Node comparableNode)
    {
        int compare = f.CompareTo(comparableNode.f);
        if (compare == 0)
        {
            compare = h.CompareTo(comparableNode.h);
        }
        return -compare;
    }
}
