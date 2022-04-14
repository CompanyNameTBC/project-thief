using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;


public class EnemyNavigation : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Rigidbody2D rb;


    public Transform goal;
    public Vector2 startingPosition;
    public int loiterTime;

    private GameObject[] goals;

    private int currentGoal;
    private String state;
    private bool waiting;
    private bool reverse;

    // Start is called before the first frame update
    void Start() {

        goals = GameObject.FindGameObjectsWithTag("enemy1Target");
        currentGoal = 0;
        startingPosition = rb.position;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.destination = goals[currentGoal].transform.position;
        waiting = false;
        state = "Patrol";
        reverse = false;
    }

    private void Update()
    {
        if(state == "Patrol")
        {
            Patrol();
        } else if (state == "Pursue")
        {
            Pursue();
        }
    }

    void Patrol()
    {
        if (waiting)
        {
            return;
        }

        if (currentGoal == -1 )
        {
            if (compare_coordinates(rb.position, startingPosition))
            {
                waiting = true;
                StartCoroutine(Wait());
            }
        } else
        {
            GameObject destination = goals[currentGoal];

            if (compare_coordinates(rb.position, destination.transform.position))
            {
                waiting = true;
                StartCoroutine(Wait());
            }
        }
    }

    void Pursue()
    { 

    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(loiterTime);
        waiting = false;
        Debug.Log(currentGoal + 1 + " : " + goals.Length);

        if(reverse == true)
        {
            if(currentGoal - 1 < 0)
            {
                reverse = false;
                currentGoal = -1;
                navMeshAgent.SetDestination(startingPosition);
            } else
            {
                currentGoal -= 1;
                navMeshAgent.SetDestination(goals[currentGoal].transform.position);
            }

        } else
        {
            if (currentGoal + 1 >= goals.Length)
            {
                reverse = true;
                Debug.Log("Returning");
                currentGoal -= 1;
                navMeshAgent.SetDestination(goals[currentGoal].transform.position);
            }
            else
            {
                currentGoal += 1;
                navMeshAgent.SetDestination(goals[currentGoal].transform.position);
            }
        }
    }

    private Boolean compare_coordinates(Vector2 vectorA, Vector2 vectorB)
    {
        return compare_floats(vectorA.x, vectorB.x) && compare_floats(vectorA.y, vectorB.y);
    }


    private Boolean compare_floats(float a, float b)
    {
        float dif = a - b;
        return dif < 1 && dif > -1;
    }

    public void SetState(String newState)
    {
        state = newState;
    }
}