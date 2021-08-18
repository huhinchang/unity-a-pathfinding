using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//https://youtu.be/-L-WgKMFuhE?t=590
public class AStarSolver : MonoBehaviour {
    class Node : IComparable<Node> {
        public Node(Vector2Int position) {
            pos = position;
        }
        public bool traversable;
        public Vector2Int pos { get; private set; }


        // distance from starting node
        private int _distFromStart;
        public int distFromStart {
            get { return _distFromStart; }
            set {
                _distFromStart = value;
                cost = value + _distToTarget;
            }

        }
        private int _distToTarget; // distance from end node
        public int distToTarget {
            get { return _distToTarget; }
            set {
                _distToTarget = value;
                cost = value + _distFromStart;
            }

        }
        private int cost; // distFromStart + distFromEnd
        public Node parent;

        public int CompareTo(Node other) {
            return (int)-(other.cost - cost);
        }
    }
    [SerializeField] Tilemap tm;
    [SerializeField] TileBase wallTile;
    [SerializeField] Vector2Int mapSize;
    [SerializeField] Vector2Int startPos;
    [SerializeField] Vector2Int targetPos;
    [SerializeField] int iterationGuard;
    Node[,] grid;

    // Start is called before the first frame update
    void Start() {
        grid = new Node[mapSize.x, mapSize.y];
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                grid[x, y] = new Node(new Vector2Int(x, y));
            }
        }
        RecalculateTraversable();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("querying pathfinder");
            iterationGuard = 100;
            Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int mousePosInt = new Vector2Int((int)mousePos.x, (int)mousePos.y);

            Debug.DrawLine((Vector2)startPos, (Vector2)mousePosInt, Color.white, 1f);

            Stack<Vector2Int> path = FindPath(startPos, mousePosInt);

            Vector2Int prev = startPos;
            while (path.Count > 0) {
                //Debug.Log(path.Peek());
                Debug.DrawLine((Vector2)prev + Vector2.one * 0.5f, (Vector2)path.Peek() + Vector2.one * 0.5f, Color.red, 1f);
                prev = path.Pop();
            }
        }
    }

    public void RecalculateTraversable() {
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                grid[x, y].traversable = (tm.GetTile(new Vector3Int(x, y, 0)) != wallTile);
            }
        }
    }

    private List<Node> GetNeighbours(Node n) {
        List<Node> temp = new List<Node>();

        Vector2Int pos = n.pos;

        if (pos.x + 1 <= mapSize.x - 1) {
            temp.Add(grid[pos.x + 1, pos.y]);
        }
        if (pos.y + 1 <= mapSize.y - 1) {
            temp.Add(grid[pos.x, pos.y + 1]);
        }
        if (pos.x - 1 >= 0) {
            temp.Add(grid[pos.x - 1, pos.y]);
        }
        if (pos.y - 1 >= 0) {
            temp.Add(grid[pos.x, pos.y - 1]);
        }

        return temp;
    }

    // returns a stack of vector2Ints, INCLUDING the start/endpoints
    Stack<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos) {

        // reset info
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                grid[x, y].distFromStart = int.MaxValue;
                grid[x, y].distToTarget = Mathf.Abs(x - targetPos.x) + Mathf.Abs(y - targetPos.y); // manhattan distance
                //grid[x, y].distToTarget  = (int) Vector2.Distance(new Vector2(x,y), targetPos);
                grid[x, y].parent = null;
            }
        }

        Stack<Vector2Int> path = new Stack<Vector2Int>();
        // a priority queue is better but unity doesn't support .NET 6 i think
        List<Node> activeNodes = new List<Node>();
        List<Node> visitedNodes = new List<Node>();

        activeNodes.Add(grid[startPos.x, startPos.y]);
        grid[startPos.x, startPos.y].parent = grid[startPos.x, startPos.y];

        // assumes every tile is reachable (requires previous flood fill)
        while (activeNodes.Count != 0 && iterationGuard > 0) {
            --iterationGuard;

            activeNodes.Sort();
            Node current = activeNodes[0];
            activeNodes.RemoveAt(0);
            visitedNodes.Add(current);

            if (current.pos == targetPos) {
                Debug.Log("Target reached! generating path...");
                while (current.pos != startPos && iterationGuard > 0) {
                    --iterationGuard;
                    path.Push(current.pos);
                    current = current.parent;
                }
                path.Push(startPos);
                return path;
            }

            List<Node> neighbours = GetNeighbours(current);

            foreach (Node n in neighbours) {
                // already visited, so move on to the next node
                if (visitedNodes.Contains(n) || !n.traversable) { continue; }

                // already in active list but we might've found a shorter path so double check
                int newDist = current.distFromStart + 1;
                if (!activeNodes.Contains(n) || newDist < n.distFromStart) {
                    n.parent = current;
                    n.distFromStart = newDist;
                    if (!activeNodes.Contains(n)) {
                        activeNodes.Add(n);
                    }
                }

            }
        }
        Debug.LogWarning("PATH NOT FOUND");
        return path;
    }

    private void OnDrawGizmos() {
        if (grid == null) { return; }
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                if (grid[x, y].traversable) {
                    Gizmos.color = Color.white;
                } else {
                    Gizmos.color = Color.red;

                }
                Gizmos.DrawWireCube(new Vector3(x + 0.5f, y + 0.5f), Vector3.one * 0.9f);
            }
        }
    }
}

