using UnityEngine;

/// <summary>
/// Utility script to automatically add colliders to all robot GameObjects.
/// Attach this to any GameObject and it will run once on Start, then disable itself.
/// </summary>
public class AddCollidersToRobots : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Automatically add colliders on Start")]
    public bool autoAddOnStart = true;

    [Tooltip("Collider size (radius for capsule, or half-extents for box)")]
    public float colliderSize = 0.5f;

    [Tooltip("Collider height (for capsule collider)")]
    public float colliderHeight = 2f;

    [Tooltip("Use Capsule Collider (better for humanoid robots)")]
    public bool useCapsule = true;

    void Start()
    {
        if (autoAddOnStart)
        {
            AddCollidersToAllRobots();
            // Disable this script after running once
            this.enabled = false;
        }
    }

    [ContextMenu("Add Colliders to All Robots")]
    public void AddCollidersToAllRobots()
    {
        int addedCount = 0;

        // Find all robots by tag
        GameObject[] taggedRobots = GameObject.FindGameObjectsWithTag("Robot");
        Debug.Log($"Found {taggedRobots.Length} robots with 'Robot' tag");

        foreach (GameObject robot in taggedRobots)
        {
            if (AddColliderToRobot(robot))
            {
                addedCount++;
            }
        }

        // Also find robots by component
        SimpleRobotMovement[] simpleRobots = FindObjectsOfType<SimpleRobotMovement>();
        foreach (SimpleRobotMovement robotScript in simpleRobots)
        {
            if (AddColliderToRobot(robotScript.gameObject))
            {
                addedCount++;
            }
        }

        RobotNavMeshMovement[] navRobots = FindObjectsOfType<RobotNavMeshMovement>();
        foreach (RobotNavMeshMovement robotScript in navRobots)
        {
            if (AddColliderToRobot(robotScript.gameObject))
            {
                addedCount++;
            }
        }

        Debug.Log($"Added colliders to {addedCount} robots!");
    }

    bool AddColliderToRobot(GameObject robot)
    {
        // Check if robot already has a collider
        Collider existingCollider = robot.GetComponent<Collider>();
        if (existingCollider != null)
        {
            Debug.Log($"Robot '{robot.name}' already has a collider: {existingCollider.GetType().Name}");
            return false;
        }

        // Add collider based on preference
        if (useCapsule)
        {
            CapsuleCollider capsule = robot.AddComponent<CapsuleCollider>();
            capsule.radius = colliderSize;
            capsule.height = colliderHeight;
            capsule.center = new Vector3(0, colliderHeight / 2f, 0); // Center at half height
            Debug.Log($"Added CapsuleCollider to '{robot.name}' (radius: {colliderSize}, height: {colliderHeight})");
        }
        else
        {
            BoxCollider box = robot.AddComponent<BoxCollider>();
            box.size = new Vector3(colliderSize * 2f, colliderHeight, colliderSize * 2f);
            box.center = new Vector3(0, colliderHeight / 2f, 0); // Center at half height
            Debug.Log($"Added BoxCollider to '{robot.name}' (size: {box.size})");
        }

        return true;
    }
}
