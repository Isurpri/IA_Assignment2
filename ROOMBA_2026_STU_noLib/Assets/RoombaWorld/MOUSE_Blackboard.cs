using UnityEngine;
using Steerings;
using Pathfinding;

public class MOUSE_Blackboard : MonoBehaviour
{
    public GameObject pooPrefab;
    public float roombaDetectionRadius = 50;

    void Awake()
    {
        pooPrefab = Resources.Load<GameObject>("POO");
    }
}
