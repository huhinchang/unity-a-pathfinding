using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour {
    [SerializeField] AStarSolver solver = null;
    [SerializeField] bool smoothing = false;
    [SerializeField] float speed = 1f;
    [SerializeField] float destinationThreshold = 0.1f;
    Queue<Vector2> path;

    private void Start() {
        path = new Queue<Vector2>();
    }

    void SmoothPath(List<Vector2Int> raw) {
        // from https://news.movel.ai/theta-star/
        Debug.Log("Smoothing...");
        Vector2 back = transform.position;
        path.Enqueue(back);
        Debug.Log($"Added {back} (start)");

        // ignore the first waypoint since it's the cell we're already in
        // ignore last waypoint because we'll always add it
        for (int i = 1; i < raw.Count - 1; ++i) {
            Vector2 candidate = raw[i] + Vector2.one * 0.5f;
            Vector2 next = raw[i+1] + Vector2.one * 0.5f;
            //path.Enqueue(candidate);
            //Debug.Log($"Added {candidate}");

            RaycastHit2D hit = Physics2D.Raycast(back, next - back, Vector2.Distance(next, back));
            if (hit.collider != null) {
                //Debug.DrawLine(back, candidate, Color.red, 3f);
                path.Enqueue(candidate);
                back = candidate;
                Debug.Log($"{candidate} is a shortcut");
            } else {
               // Debug.DrawLine(back, candidate, Color.blue, 3f);
            }

        }

        Debug.Log($"Added {raw[raw.Count - 1]} (end)");
        path.Enqueue(raw[raw.Count - 1] + Vector2.one * 0.5f);
    }
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("querying pathfinder");
            Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int mousePosInt = new Vector2Int((int)mousePos.x, (int)mousePos.y);
            Vector2Int myPosInt = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            //Debug.DrawLine(transform.position, (Vector2)mousePosInt, Color.white, 1f);

            List<Vector2Int> raw = solver.FindPath(myPosInt, mousePosInt);
            path.Clear();
            if (raw.Count > 0) {
                if (smoothing) {
                    SmoothPath(raw);
                } else {
                    foreach (Vector2Int v in raw) {
                        path.Enqueue(v + Vector2.one * 0.5f);
                    }
                }
            }

            for (int i = 0; i < raw.Count - 1; i++) {
                Debug.DrawLine((Vector2)raw[i] + Vector2.one * 0.6f, raw[i + 1] + Vector2.one * 0.6f, Color.yellow, 3f);
            }
            /*
            prev = transform.position;
            foreach (Vector2 v in path) {
                Debug.DrawLine(prev + Vector2.one * 0.5f, v + Vector2.one * 0.5f, Color.red, 3f);
                prev = v;
            }
            */
        }


        if (path.Count > 0) {
            transform.Translate((path.Peek() - (Vector2)transform.position).normalized * speed * Time.deltaTime);
            if (Vector2.Distance(path.Peek(), transform.position) < destinationThreshold) {
                path.Dequeue();
            }
        }

    }

    private void OnDrawGizmos() {
    /*
        if (path != null && path.Count > 0)
            Gizmos.DrawLine(transform.position, path.Peek());
            */
    }
}
