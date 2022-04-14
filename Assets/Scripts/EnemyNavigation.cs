using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyNavigation : MonoBehaviour
{
    public NavMeshAgent agent;
    public Rigidbody2D rb;
    public Transform player;

    public Vector2 startingPosition;
    public int loiterTime;

    private Transform[] goals;
    private int currentGoal;

    private String state;
    private bool waiting;
    private bool reverse;

    // Start is called before the first frame update
    void Start() {
        // goals = GameObject.FindGameObjectsWithTag(tag);


        Transform[] path = transform.parent.Find("Path").GetComponentsInChildren<Transform>();

        goals = path.Where(child => child.CompareTag("target")).ToArray();
        currentGoal = 0;
        startingPosition = getPosition();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.destination = getTargetPosition();

        waiting = false;
        state = "Patrol";
        reverse = false;
    }

    private void Update()
    {
        switch (state)
        {
            case "Pursue":
                Pursue();
                break;
            default:
                Patrol();
                break;
        }
    }

    void Patrol()
    {
        Vector2 nextPosition = getTargetPosition();
        AdjustAvatar(nextPosition);
        if (waiting)
        {
            return;
        }
       if (CoordinatesMatch(getPosition(), nextPosition))
       {
         waiting = true;
         StartCoroutine(LoiterAndSetNextDestination());
       }  
    }

    void Pursue()
    {
        AdjustAvatar(player.position);

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

        agent.SetDestination(getTargetPosition());
    }

    private Vector2 getPosition()
    {
        return transform.localPosition;
    }

    private Vector2 getTargetPosition()
    {
        return goals[currentGoal].transform.localPosition;
    }

    private void AdjustAvatar(Vector2 target)
    {
        if (getPosition().x < target.x)
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
        transform.localScale = new Vector3(
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