using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour {
    [SerializeField] AStarSolver solver = null;
    [SerializeField] bool smoothing = false;
    [SerializeField] float speed = 1f;
    [SerializeField] float destinationThreshold = 0.1f;
    Stack<Vector2Int> path;
    Vector2 nextWaypoint;

    private void Start() {
        path = new Stack<Vector2Int>();
        nextWaypoint = transform.position;
    }

    void getNextWaypoint() {
        if (path.Count > 0) {
            nextWaypoint = path.Pop() + Vector2.one * 0.5f;
        }

        if (smoothing) {
            while (path.Count > 0) {
                Vector2 candidate = path.Peek() + Vector2.one * 0.5f;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, candidate - (Vector2)transform.position, Vector2.Distance(candidate, transform.position));
                if (hit.collider == null) {
                    nextWaypoint = candidate;
                    Debug.Log($"{candidate} is a shortcut");
                    path.Pop();
                } else {
                    Debug.Log($"{candidate} is NOT a shortcut");
                    break;
                }
            }
        }
    }
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("querying pathfinder");
            Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int mousePosInt = new Vector2Int((int)mousePos.x, (int)mousePos.y);
            Vector2Int myPosInt = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            //Debug.DrawLine(transform.position, (Vector2)mousePosInt, Color.white, 1f);

            path = solver.FindPath(myPosInt, mousePosInt);
            //path.Pop(); // pop the first waypoint since it's the cell we're already in
            getNextWaypoint();
            /*
            Vector2 prev = transform.position;
            while (path.Count > 0) {
                //Debug.Log(path.Peek());
                Debug.DrawLine((Vector2)prev + Vector2.one * 0.5f, (Vector2)path.Peek() + Vector2.one * 0.5f, Color.red, 1f);
                prev = path.Pop();
            }
            */
        }


        if (path.Count > 0 && Vector2.Distance(nextWaypoint, transform.position) < destinationThreshold) {
            Debug.Log("Going to next waypoint");
            getNextWaypoint();
        }


        transform.Translate((nextWaypoint - (Vector2)transform.position).normalized * speed * Time.deltaTime);


    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, nextWaypoint);
    }
}
