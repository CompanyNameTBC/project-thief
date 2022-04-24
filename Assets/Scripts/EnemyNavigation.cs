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

    [SerializeField] private Transform pfFieldOfView;
    private FieldOfView fieldOfView;
    [SerializeField] private float fov;
    [SerializeField] private float viewDistance;

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

        fieldOfView = Instantiate(pfFieldOfView, null).GetComponent<FieldOfView>();
        fieldOfView.SetFOV(fov);
        fieldOfView.SetViewDistance(viewDistance);
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
                FindTargetPlayer();
                break;
        }

        if (fieldOfView != null)
        {
            fieldOfView.SetOrigin(getCurrentPosition());
            fieldOfView.SetAimDirection(transform.localScale);
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

    private void FindTargetPlayer()
    {
        // TODO
        // Needs a Vector3 represents the way the enemy is facing, according to tutorial
        Vector3 aimDirection = Vector3.forward; // placeholder for the above
        if (Vector2.Distance(getCurrentPosition(), player.localPosition) < viewDistance)
        {
            // Player is inside the view distance

            Vector2 directionToPlayer = (player.localPosition - transform.localPosition).normalized;
            if (Vector2.Angle(aimDirection, directionToPlayer) < fov/2f)
            {
                // Player is inside the field of view

                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.localPosition, directionToPlayer, viewDistance);
                if (raycastHit2D.collider != null)
                {
                    // Hit something/can see something
                    if (raycastHit2D.collider.gameObject.name.Equals("Player"))
                    {
                        // Hit player
                        ActivatePursuePlayer();
                        Debug.Log("PLAYER HIT");
                    }
                    else
                    {
                        // Hit something else
                        ActivatePatrol();
                        Debug.Log("NOTHING");
                    }
                }
            }
        }
    }
}