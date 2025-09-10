using UnityEngine;

public class RemoveBlockingColliders : MonoBehaviour
{
    [Header("Remove Colliders")]
    public string[] objectNamesToKeepColliders = {"floor", "ground", "platform", "stairs"}; // Keep floor colliders
    
    [ContextMenu("Remove Blocking Colliders")]
    public void RemoveColliders()
    {
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        int removedCount = 0;
        
        foreach (Collider col in allColliders)
        {
            string objName = col.gameObject.name.ToLower();
            
            // Keep floor/ground colliders for walking
            bool shouldKeep = false;
            foreach (string keepName in objectNamesToKeepColliders)
            {
                if (objName.Contains(keepName.ToLower()))
                {
                    shouldKeep = true;
                    break;
                }
            }
            
            // Remove wall/window/ceiling colliders that block movement
            if (!shouldKeep)
            {
                Debug.Log($"Removing collider from: {col.gameObject.name}");
                DestroyImmediate(col);
                removedCount++;
            }
        }
        
        Debug.Log($"Removed {removedCount} blocking colliders, kept floor colliders");
    }
}