using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class EnemyNavigation : MonoBehaviour
{
    public NavMeshAgent agent;
    public Rigidbody2D rb;

    public Transform player;

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

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.destination = goals[currentGoal].transform.position;

        waiting = false;
        state = "Pursue";
        reverse = false;
    }

    private void Update()
    {
        switch (state)
        {
            case "Pursue": Pursue();
                break;
            default: Patrol();
                break;
        }
    }

    void Patrol()
    {
        Vector2 nextPosition = goals[currentGoal].transform.position;

        AdjustAvatar(nextPosition);
        if (waiting)
        {
            return;
        }
       Vector3 goal = goals[currentGoal].transform.position; 
       if (CoordinatesMatch(rb.position, goal))
       {
         waiting = true;
         StartCoroutine(LoiterAndSetNextDestination());
       }  
    }

    void Pursue()
    {
        AdjustAvatar(player.transform.position);

        agent.destination = player.position;
    }

    IEnumerator LoiterAndSetNextDestination()
    {
        yield return new WaitForSeconds(loiterTime);
        waiting = false;

        if (reverse)
        {
            if(currentGoal == 0)
            {
                reverse = false;
            } 
        } else
        {
            if (currentGoal == goals.Length -1)
            {
                reverse = true;
            }
        }

        currentGoal = reverse ? currentGoal - 1 : currentGoal + 1;

        agent.SetDestination(goals[currentGoal].transform.position);
    }

    private void AdjustAvatar(Vector2 target)
    {
        if (gameObject.transform.position.x < target.x)
        {
            //Right
            AdjustRotation(1);
        }
        else
        {
            //Left
            AdjustRotation(-1);
        }
    }

    private void AdjustRotation(int adjustment)
    {
        Transform t = this.gameObject.transform;
        t.localScale = new Vector3(
          adjustment,
          transform.localScale.y,
          transform.localScale.z);
    }

    private Boolean CoordinatesMatch(Vector2 vectorA, Vector2 vectorB)
    {
        return FloatsAlmostMatch(vectorA.x, vectorB.x) && FloatsAlmostMatch(vectorA.y, vectorB.y);
    }


    private Boolean FloatsAlmostMatch(float a, float b)
    {
        float dif = a - b;
        return dif < 1 && dif > -1;
    }

    public void ActivatePursuePlayer()
    {
        SetState("Pursue");
    }

    public void ActivatePatrol()
    {
        SetState("Patrol");
    }

    private void SetState(String newState)
    {
        state = newState;
    }
}