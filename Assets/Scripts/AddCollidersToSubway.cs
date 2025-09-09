using UnityEngine;

public class AddCollidersToSubway : MonoBehaviour
{
    [Header("Add Colliders")]
    public bool addToChildren = true;
    public bool useMeshCollider = true;
    public bool makeConvex = false; // Keep false for static geometry
    
    [ContextMenu("Add Colliders to All")]
    public void AddCollidersToAll()
    {
        if (addToChildren)
        {
            // Get all child objects with MeshRenderer
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            
            foreach (MeshRenderer renderer in renderers)
            {
                AddColliderToObject(renderer.gameObject);
            }
            
            Debug.Log($"Added colliders to {renderers.Length} subway parts");
        }
        else
        {
            AddColliderToObject(gameObject);
        }
    }
    
    void AddColliderToObject(GameObject obj)
    {
        // Skip if already has a collider
        if (obj.GetComponent<Collider>() != null)
        {
            Debug.Log($"Skipping {obj.name} - already has collider");
            return;
        }
        
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogWarning($"Skipping {obj.name} - no mesh found");
            return;
        }
        
        if (useMeshCollider)
        {
            MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
            meshCollider.convex = makeConvex;
            Debug.Log($"Added MeshCollider to {obj.name}");
        }
        else
        {
            // Use BoxCollider as fallback
            obj.AddComponent<BoxCollider>();
            Debug.Log($"Added BoxCollider to {obj.name}");
        }
    }
    
    [ContextMenu("Remove All Colliders")]
    public void RemoveAllColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        
        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject) // Don't remove from this script's object
            {
                DestroyImmediate(col);
            }
        }
        
        Debug.Log($"Removed {colliders.Length} colliders");
    }
}