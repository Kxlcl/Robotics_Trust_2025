using UnityEngine;
using UnityEngine.SceneManagement;

public class SharedMapManager : MonoBehaviour
{
    [System.Serializable]
    public struct SceneArea
    {
        public string sceneName;
        public Vector3 spawnPosition;
        public Vector3 spawnRotation;
    }

    public SceneArea[] sceneAreas;
    public FirstPersonController playerController;
    
    private string currentArea;

    void Start()
    {
        // Keep this object alive between scene loads
        DontDestroyOnLoad(gameObject);
        
        // Start with first area if none specified
        if (sceneAreas.Length > 0)
            LoadArea(sceneAreas[0].sceneName);
    }

    public void LoadArea(string areaName)
    {
        // Unload previous area scene if exists
        if (!string.IsNullOrEmpty(currentArea) && currentArea != areaName)
        {
            SceneManager.UnloadSceneAsync(currentArea);
        }

        // Find the area configuration
        SceneArea targetArea = System.Array.Find(sceneAreas, area => area.sceneName == areaName);
        
        if (targetArea.sceneName != null)
        {
            // Move player to spawn position
            if (playerController != null)
            {
                playerController.transform.position = targetArea.spawnPosition;
                playerController.transform.eulerAngles = targetArea.spawnRotation;
            }

            // Load area scene additively
            SceneManager.LoadSceneAsync(areaName, LoadSceneMode.Additive);
            currentArea = areaName;
        }
    }

    public void TransitionToArea(string areaName)
    {
        LoadArea(areaName);
    }
}