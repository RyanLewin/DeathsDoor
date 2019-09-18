using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    TileGrid tileGrid;
    PathRequestManager requestManager;

    public Heap<Node> openNodes;
    public HashSet<Node> closedNodes;

    public Node currentNode;

    int maxSize;

    // Start is called before the first frame update
    void Awake()
    {
        tileGrid = TileGrid.GetGrid;
        requestManager = GetComponent<PathRequestManager>();
        maxSize = tileGrid.xSize * tileGrid.ySize;
    }

    public void StartFindPath (Vector3 start, Vector3 end)
    {
        StartCoroutine(FindPath(start, end));
    }

    IEnumerator FindPath(Vector3 _start, Vector3 _end)
    {

        //if they're on the same node, then return the nodes position

        Vector3[] path = new Vector3[0];
        bool pathSuccess = false;

        Node start = tileGrid.GetTileAtPos(_start).GetComponent<Node>();
        Node target = tileGrid.GetTileAtPos(_end).GetComponent<Node>();
        if (!start || !target)
            yield return null;

        if (start != target)
        {
            //list of open and closed nodes
            openNodes = new Heap<Node>(tileGrid.xSize * tileGrid.ySize);
            closedNodes = new HashSet<Node>();

            openNodes.Add(start);

            int x = 0;
            while (openNodes.Count > 0)
            {
                currentNode = openNodes.RemoveFirst();
                closedNodes.Add(currentNode);

                if (currentNode == target)
                {
                    pathSuccess = true;
                    break; 
                }

                foreach (Node n in tileGrid.GetNeighbours(currentNode))
                {
                    if (closedNodes.Contains(n))
                        continue;
                    if (!n.walkable)
                        continue;

                    int f = currentNode.g + GetDistance(currentNode, n) + n.weight;
                    if (f < n.g || !openNodes.Contains(n))
                    {
                        n.g = f;
                        n.h = GetDistance(n, target);
                        n.ParentNode = currentNode;

                        if (!openNodes.Contains(n))
                        {
                            openNodes.Add(n);
                        }
                    }
                    x++;
                }

                if (closedNodes.Count >= maxSize - 1)
                {
                    currentNode = null;
                    break;
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            path = RetracePath(start, target);
        }
        requestManager.FinishedProcessing(path, pathSuccess);
    }

    Vector3[] RetracePath (Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 oldDir = Vector2.zero;

        Vector3 pathPos = path[0].transform.position;
        pathPos.z = -0.01f;
        waypoints.Add(pathPos);
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 newDir = new Vector2(path[i - 1].x - path[i].x, path[i - 1].y - path[i].y);
            if (newDir != oldDir)
            {
                pathPos = path[i - 1].transform.position;
                pathPos.z = -0.01f;
                waypoints.Add(pathPos);
            }
            oldDir = newDir;
        }
        waypoints.Add(path[path.Count - 1].transform.position);
        return waypoints.ToArray();
    }

    int GetDistance (Node a, Node b)
    {
        int distX = Mathf.Abs(a.x - b.x);
        int  distY = Mathf.Abs(a.y - b.y);

        if (distX > distY)
            return 4 * distY + 10 * distX;
        return 4 * distX +  10 * distY;
    }
}


//helper struct to pair total weight and node
//public struct NodeValue : IHeapItem<NodeValue>
//{
//    public Node Node;
//    public float g;
//    public bool walkable;

//    public NodeValue (Node _node, float _weight, bool _walkable)
//    {
//        Node = _node;
//        g = _weight;
//        walkable = _walkable;
//    }

//    int heapIndex;
//    public int HeapIndex
//    {
//        get { return heapIndex; }
//        set { heapIndex = value; }
//    }

//    public int CompareTo (NodeValue comparableNode)
//    {
//        int compare = Node.f.CompareTo(comparableNode.Node.f);
//        if (compare == 0)
//        {
//            compare = Node.h.CompareTo(comparableNode.Node.h);
//        }
//        return -compare;
//    }
//}
