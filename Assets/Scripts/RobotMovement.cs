using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform targetPosition;
    public float moveSpeed = 2f;
    public bool startMoving = false;
    
    [Header("Optional: Look At Target")]
    public bool lookAtTarget = true;
    
    private bool hasReachedTarget = false;
    
    void Update()
    {
        if (startMoving && targetPosition != null && !hasReachedTarget)
        {
            MoveToTarget();
        }
    }
    
    void MoveToTarget()
    {
        // Calculate direction to target
        Vector3 direction = (targetPosition.position - transform.position).normalized;
        
        // Look at target if enabled
        if (lookAtTarget && direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
        }
        
        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);
        
        // Check if reached target
        if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
        {
            hasReachedTarget = true;
            OnReachedTarget();
        }
    }
    
    void OnReachedTarget()
    {
        Debug.Log($"{gameObject.name} reached target position");
        // Add any actions you want when robot reaches destination
    }
    
    public void StartWalking()
    {
        startMoving = true;
        hasReachedTarget = false;
    }
    
    public void StopWalking()
    {
        startMoving = false;
    }
    
    public void SetTarget(Transform newTarget)
    {
        targetPosition = newTarget;
        hasReachedTarget = false;
    }
}