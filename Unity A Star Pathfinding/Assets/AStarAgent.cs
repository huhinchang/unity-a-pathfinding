using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
}
