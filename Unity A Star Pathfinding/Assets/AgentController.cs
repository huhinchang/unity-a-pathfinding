using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField] AStarSolver solver = null;
    [SerializeField] AStarAgent agent = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.stopped) {
            agent.GoTo(new Vector2(Random.Range(0, solver.mapSize.x), Random.Range(0, solver.mapSize.y)));
        }
    }
}
