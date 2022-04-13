using UnityEngine;
using UnityEngine.AI;
using System;



public class Navigation : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform goal;
    public Vector2 startingPosition;
    public Rigidbody2D rb;
    public bool returning;

    // Start is called before the first frame update
    void Start() { 
        startingPosition = rb.position;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.destination = goal.position;
        returning = false;
    }

    private void Update()
    {
        float goalX = goal.position.x;
        float goalY = goal.position.y;
        float actX = rb.position.x;
        float actY = rb.position.y;
        float startX = startingPosition.x;
        float startY = startingPosition.y;

        if (returning == false && (compare_floats(actX, goalX) && compare_floats(actY, goalY)))
        {
            returning = true;
            navMeshAgent.SetDestination(startingPosition);
        }
        else if (returning == true && (compare_floats(actX, startX) && compare_floats(actY, startY)))
        {
            returning = false;
            navMeshAgent.SetDestination(goal.position);
        }
    }


    private Boolean compare_floats(float a, float b)
    {
        float dif = a - b;
        return dif < 1 && dif > -1;
    }
}