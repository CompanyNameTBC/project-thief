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

    // Start is called before the first frame update
    void Start() {
        targetPath = getTargetPath();

        currentTarget = 0;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.destination = getTargetPosition();

        state = "Patrol";
    }

    void Update()
    {
        switch (state)
        {
            case "Pursue":
                Pursue();
                break;
            case "Patrol":
                Patrol();
                break;
            default:
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

    void ActivateLoiter(){
        SetState("Loiter");
    }


    void Patrol()
    {
        Vector2 targetPosition = getTargetPosition();

        FaceTarget(targetPosition);

       if (CoordinatesMatch(getCurrentPosition(), targetPosition))
       {
           ActivateLoiter();
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

        currentTarget = currentTarget >= ( targetPath.Length - 1 ) ? 0 : currentTarget + 1;

        Debug.Log(currentTarget);

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