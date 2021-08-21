using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour {
    [SerializeField] AStarSolver solver = null;
    [SerializeField] bool smoothing = false;
    [SerializeField] bool jitter = false;
    [SerializeField] float speed = 1f;
    [SerializeField] float destinationThreshold = 0.1f;
    public bool stopped {
        get {
            return path.Count == 0;
        }
    }
    List<Vector2> path;

    private void Start() {
        path = new List<Vector2>();
    }

    void SmoothPath() {
        // from https://news.movel.ai/theta-star/
        Debug.Log("Smoothing...");

        //Vector2 back = transform.position;
        //path.Add(back);
        //Debug.Log($"Added {back} (start)");

        // ignore the first waypoint since it's the cell we're already in
        // ignore last waypoint because we'll always add it
        Vector2 origin = path[0];
        int i = 1;
        while (i < path.Count - 1) {
            Vector2 candidate = path[i];
            Vector2 next = path[i + 1];
            //path.Enqueue(candidate);
            //Debug.Log($"Added {candidate}");

            RaycastHit2D hit = Physics2D.Raycast(origin, next - origin, Vector2.Distance(next, origin));
            if (hit.collider != null) {
                //Debug.DrawLine(origin, candidate, Color.red, 3f);
                //path.Add(candidate);
                origin = path[i];
                ++i;
                //Debug.Log($"{candidate} is necessary");
            } else {
                //Debug.DrawLine(origin, candidate, Color.blue, 3f);
                path.RemoveAt(i);
                //Debug.Log($"{candidate} can be skipped");
            }

        }

        //Debug.Log($"Added {raw[raw.Count - 1]} (end)");
        //path.Enqueue(raw[raw.Count - 1] + Vector2.one * 0.5f);
    }

    public void GoTo(Vector2 destination) {
        Debug.Log("querying pathfinder");
        Vector2Int destInt = new Vector2Int((int)destination.x, (int)destination.y);
        Vector2Int myPosInt = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        List<Vector2Int> raw = solver.FindPath(myPosInt, destInt);
        path.Clear();
        if (raw.Count > 0) {
            Vector2 offset;
            if (jitter) {
                offset = (Vector2)transform.position - myPosInt;
            } else {
                offset = Vector2.one * 0.5f;
            }
            foreach (Vector2Int v in raw) {
                path.Add(v + offset);
            }
            if (smoothing) {
                SmoothPath();
            }
        }

        for (int i = 0; i < raw.Count - 1; i++) {
            Debug.DrawLine((Vector2)raw[i] + Vector2.one * 0.6f, raw[i + 1] + Vector2.one * 0.6f, Color.yellow, 3f);
        }

        Vector2 prev = transform.position;
        foreach (Vector2 v in path) {
            Debug.DrawLine(prev, v, Color.red, 3f);
            prev = v;
        }
    }
    private void Update() {
        if (path.Count > 0) {
            Vector2 destination = path[0];
            transform.Translate((destination - (Vector2)transform.position).normalized * speed * Time.deltaTime);
            if (Vector2.Distance(destination, transform.position) < destinationThreshold) {
                path.RemoveAt(0);
            }
        }

    }
}
