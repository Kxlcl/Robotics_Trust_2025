using UnityEngine;

public class AreaTransitionTrigger : MonoBehaviour
{
    [Header("Area Transition")]
    public string targetAreaName;
    public bool showTransitionMessage = true;
    public string transitionMessage = "Press F to enter";
    
    private bool playerInRange = false;
    private SharedMapManager mapManager;

    void Start()
    {
        mapManager = FindObjectOfType<SharedMapManager>();
        if (mapManager == null)
            Debug.LogError("SharedMapManager not found in scene!");
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            TransitionToArea();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (showTransitionMessage)
            {
                // Show UI message (you can integrate with your existing UI system)
                Debug.Log(transitionMessage);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void TransitionToArea()
    {
        if (mapManager != null)
        {
            mapManager.TransitionToArea(targetAreaName);
        }
    }

    void OnGUI()
    {
        if (playerInRange && showTransitionMessage)
        {
            GUI.Label(new Rect(Screen.width/2 - 100, Screen.height - 100, 200, 50), transitionMessage);
        }
    }
}