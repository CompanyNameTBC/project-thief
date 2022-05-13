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

    private EnemyState enemyState;

    // Field of View variables
    [SerializeField] private Transform pfFieldOfView;
    private FieldOfView fieldOfView;
    [SerializeField] private float fov;
    [SerializeField] private float viewDistance;
    private float attackDistanceFraction;

    private Vector2 playerLastKnownPosition;

    // Start is called before the first frame update
    void Start() {
        targetPath = getTargetPath();

        currentTarget = 0;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.destination = getTargetPosition();

        enemyState = EnemyState.Patrol;

        // Initialize the FieldOfView prefab and set the fov angle and view distance
        fieldOfView = Instantiate(pfFieldOfView, null).GetComponent<FieldOfView>();
        fieldOfView.SetFOV(fov);
        fieldOfView.SetViewDistance(viewDistance);

        // Set the enemy attack distance as a percentage of the viewDistance
        attackDistanceFraction = 0.5f;

        playerLastKnownPosition = player.position;
    }

    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Pursue:
                Pursue();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            default:
                break;
        }

        if (fieldOfView != null)
        {
            fieldOfView.SetOrigin(getCurrentPosition());
            fieldOfView.SetAimDirection(transform.localScale);
            FindTargetPlayer();
        }
    }

    void ActivatePursuePlayer()
    {
        SetEnemyState(EnemyState.Pursue);
    }

    void ActivatePatrol()
    {
        SetEnemyState(EnemyState.Patrol);
    }

    void ActivateLoiter(){
        SetEnemyState(EnemyState.Loiter);
    }


    private void Patrol()
    {
        Vector2 targetPosition = getTargetPosition();

        FaceTarget(targetPosition);

       if (CoordinatesMatch(getCurrentPosition(), targetPosition))
       {
           ActivateLoiter();
           StartCoroutine(LoiterAndSetNextDestination());
       }  
    }

    private void Pursue()
    {
        FaceTarget(playerLastKnownPosition);

        agent.destination = playerLastKnownPosition;

        // If the enemy has reached the player's last known position, loiter then continue patrol
        if (CoordinatesMatch(getCurrentPosition(), playerLastKnownPosition))
        {
            ActivateLoiter();
            StartCoroutine(LoiterAndSetNextDestination());
        }
    }

    private IEnumerator LoiterAndSetNextDestination()
    {
        yield return new WaitForSeconds(loiterTime);

        currentTarget = currentTarget >= ( targetPath.Length - 1 ) ? 0 : currentTarget + 1;

        agent.SetDestination(getTargetPosition());
        
        ActivatePatrol();
    }


    private Transform[] getTargetPath() {
        return transform.parent.Find("Path")
            .GetComponentsInChildren<Transform>()
            .Where(child => child.CompareTag("target"))
            .ToArray();
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
        transform.localScale = new Vector3(adjustment, transform.localScale.y, transform.localScale.z);
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

    private void SetEnemyState(EnemyState newState)
    {
        enemyState = newState;
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
                        // Go to the player's last known position
                        playerLastKnownPosition = player.position;
                        ActivatePursuePlayer();
                        
                        if (Vector2.Distance(getCurrentPosition(), player.localPosition) < (viewDistance * attackDistanceFraction))
                        {
                            Debug.Log("PLAYER ATTACKED");
                        }
                    }
                    else
                    {
                        // Hit something else
                        ActivatePatrol();
                        //Debug.Log("NOTHING");
                    }
                }
            }
        }
    }
}