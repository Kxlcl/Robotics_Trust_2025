using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotNavMeshMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform targetPosition;
    public bool startMoving = false;
    
    private NavMeshAgent agent;
    private bool hasReachedTarget = false;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component required!");
        }
    }
    
    void Update()
    {
        if (startMoving && targetPosition != null && agent != null)
        {
            // Set destination
            if (!hasReachedTarget)
            {
                agent.SetDestination(targetPosition.position);
            }
            
            // Check if reached destination
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                if (!hasReachedTarget)
                {
                    hasReachedTarget = true;
                    OnReachedTarget();
                }
            }
        }
    }
    
    void OnReachedTarget()
    {
        Debug.Log($"{gameObject.name} reached target position via NavMesh");
        agent.isStopped = true;
        // Add any actions you want when robot reaches destination
    }
    
    public void StartWalking()
    {
        if (agent != null)
        {
            startMoving = true;
            hasReachedTarget = false;
            agent.isStopped = false;
        }
    }
    
    public void StopWalking()
    {
        startMoving = false;
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }
    
    public void SetTarget(Transform newTarget)
    {
        targetPosition = newTarget;
        hasReachedTarget = false;
        
        if (startMoving && agent != null)
        {
            agent.SetDestination(targetPosition.position);
        }
    }
    
    public void SetTarget(Vector3 newTargetPosition)
    {
        hasReachedTarget = false;
        
        if (startMoving && agent != null)
        {
            agent.SetDestination(newTargetPosition);
        }
    }
}