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

    public int loiterTime;

    private Transform[] targetPath;
    private int currentTarget;

    private String state;
    private bool loitering;
    private bool reverse;

    // Start is called before the first frame update
    void Start() {
        targetPath = transform.parent.Find("Path")
            .GetComponentsInChildren<Transform>()
            .Where(child => child.CompareTag("target"))
            .ToArray();

        currentTarget = 0;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.destination = getTargetPosition();

        state = "Patrol";
        reverse = false;
        loitering = false;
    }

    void Update()
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

    void ActivatePursuePlayer()
    {
        SetState("Pursue");
    }

    void ActivatePatrol()
    {
        SetState("Patrol");
    }


    void Patrol()
    {
        if (loitering)
        {
            return;
        }

        Vector2 targetPosition = getTargetPosition();

        FaceTarget(targetPosition);

       if (CoordinatesMatch(getCurrentPosition(), targetPosition))
       {
           loitering = true;
           StartCoroutine(LoiterAndSetNextDestination());
       }  
    }

    void Pursue()
    {
        FaceTarget(player.position);

        agent.destination = player.position;
    }

    IEnumerator LoiterAndSetNextDestination()
    {
        yield return new WaitForSeconds(loiterTime);
        loitering = false;

        if (reverse)
        {
            if(currentTarget == 0)
            {
                reverse = false;
            } 
        } else
        {
            if (currentTarget == targetPath.Length -1)
            {
                reverse = true;
            }
        }

        currentTarget = reverse ? currentTarget - 1 : currentTarget + 1;

        agent.SetDestination(getTargetPosition());
    }

    private Vector2 getCurrentPosition()
    {
        return transform.localPosition;
    }

    private Vector2 getTargetPosition()
    {
        return targetPath[currentTarget].transform.localPosition;
    }

    private void FaceTarget(Vector2 target)
    {
        if (getCurrentPosition().x < target.x)
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

    private void SetState(String newState)
    {
        state = newState;
    }
}