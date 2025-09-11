using UnityEngine;

public class SimpleRobotMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform targetPosition;
    public float moveSpeed = 2f;
    public bool startMoving = false;
    
    [Header("Movement Options")]
    public bool lockY = true; // Keep robot at same height
    public bool lookAtTarget = true;
    
    private bool hasReachedTarget = false;
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        if (startMoving && targetPosition != null && !hasReachedTarget)
        {
            MoveHorizontally();
        }
    }
    
    void MoveHorizontally()
    {
        // Get target position but keep original Y (height)
        Vector3 target = targetPosition.position;
        if (lockY)
        {
            target.y = startPosition.y; // Keep robot at original height
        }
        
        // Calculate horizontal direction
        Vector3 direction = (target - transform.position).normalized;
        
        // Look at target if enabled (only horizontal rotation)
        if (lookAtTarget && direction != Vector3.zero)
        {
            direction.y = 0; // Remove vertical component for rotation
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
            }
        }
        
        // Move towards target horizontally
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        
        // Check if reached target (horizontal distance only)
        Vector3 horizontalDistance = target - transform.position;
        horizontalDistance.y = 0; // Ignore Y difference
        
        if (horizontalDistance.magnitude < 0.1f)
        {
            hasReachedTarget = true;
            OnReachedTarget();
        }
    }
    
    void OnReachedTarget()
    {
        Debug.Log($"{gameObject.name} reached horizontal target position");
        startMoving = false;
    }
    
    public void StartWalking()
    {
        startMoving = true;
        hasReachedTarget = false;
        startPosition = transform.position; // Update start position
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
    
    public void SetTarget(Vector3 newTargetPosition)
    {
        // Create a temporary target position
        GameObject tempTarget = new GameObject("TempTarget");
        tempTarget.transform.position = newTargetPosition;
        targetPosition = tempTarget.transform;
        hasReachedTarget = false;
    }
}