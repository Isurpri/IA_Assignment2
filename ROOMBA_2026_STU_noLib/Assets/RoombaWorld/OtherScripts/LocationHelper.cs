using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class LocationHelper  {

    private static List<GraphNode> allNodes;
    private static List<GameObject> patrolPoints;
    private static List<GameObject> entryExitPoints;
    
    static LocationHelper ()
    {
        // get all the nodes in the gridgraph and save the walkable ones in allNodes list.
        allNodes = new List<GraphNode>();
        GridGraph gg = AstarPath.active.data.gridGraph;
        gg.GetNodes(nod => { if (nod.Walkable) allNodes.Add(nod); });

        // get all the patrol points
        patrolPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("PATROLPOINT"));
        
        // get all the entry&exit points
        entryExitPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("ENTRYEXITPOINT"));
    } 

    public static Vector3 RandomWalkableLocation ()
    {
        GraphNode node = allNodes[Random.Range(0, allNodes.Count)];
        // return its position as a vector 3
        return (Vector3)node.position;
    }

    public static GameObject RandomPatrolPoint ()
    {
        return patrolPoints[Random.Range(0, patrolPoints.Count)];
    }
    
    public static GameObject RandomEntryExitPoint ()
    {
        return entryExitPoints[Random.Range(0, entryExitPoints.Count)];
    }
    
    public static GameObject NearestExitPoint (GameObject gameObject)
    {
        GameObject nearest = entryExitPoints[0];
        float best = SensingUtils.DistanceToTarget(gameObject, nearest);
        float current;
        // process all exit points. Retain the nearest
        for (int i=1; i<entryExitPoints.Count; i++)
        {
            current = SensingUtils.DistanceToTarget(gameObject, entryExitPoints[i]);
            if (current<best)
            {
                best = current;
                nearest = entryExitPoints[i];
            }
        }
        return nearest;
    }

}
