using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GlobalTimer : MonoBehaviour
{
    public static GlobalTimer Instance;
    private float initialTime = 600f; // 10 minutes
    public float timeRemaining;
    public TextMeshProUGUI timerText; // Assign in Inspector
    public bool timerStarted = false;

    void Awake()
    {
        // Singleton pattern to prevent multiple timers
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            timeRemaining = initialTime;
            Debug.Log("GlobalTimer singleton created and marked DontDestroyOnLoad");
        }
        else
        {
            // Destroy duplicate timer
            Debug.Log("Duplicate GlobalTimer destroyed");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Find timer UI element in current scene
        FindTimerUI();
        // Check if we should start the timer
        CheckSceneAndStartTimer();
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find timer UI element in new scene
        FindTimerUI();
        CheckSceneAndStartTimer();
    }
    
    public void FindTimerUI()
    {
        // Find timer UI element in current scene
        TextMeshProUGUI[] allTimerTexts = FindObjectsOfType<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in allTimerTexts)
        {
            // Look for timer UI by name or tag - assuming it's named "Timer" or has specific properties
            if (text.name.ToLower().Contains("timer") || text.gameObject.name.ToLower().Contains("timer"))
            {
                timerText = text;
                Debug.Log($"Found timer UI: {text.name} in {SceneManager.GetActiveScene().name}");
                break;
            }
        }
        
        // If we still don't have a timer reference, try to find by GameObject name pattern
        if (timerText == null)
        {
            GameObject timerObj = GameObject.Find("Timer");
            if (timerObj == null) timerObj = GameObject.Find("TimerText");
            if (timerObj == null) timerObj = GameObject.Find("TimeDisplay");
            
            if (timerObj != null)
            {
                timerText = timerObj.GetComponent<TextMeshProUGUI>();
                if (timerText != null)
                {
                    Debug.Log($"Found timer UI by GameObject search: {timerObj.name}");
                }
            }
        }
        
        if (timerText == null)
        {
            Debug.LogWarning($"Could not find timer UI in {SceneManager.GetActiveScene().name}");
        }
    }

    void CheckSceneAndStartTimer()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "ID_Scene" || currentScene == "Waiting_Scene")
        {
            // Show timer if already started
            if (timerText != null && timerStarted)
            {
                timerText.gameObject.SetActive(true);
            }
        }
        else
        {
            // Hide timer in other scenes
            if (timerText != null)
            {
                timerText.gameObject.SetActive(false);
            }
        }
    }
    
    void StartTimer()
    {
        timerStarted = true;
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            // Force text color to be fully opaque white
            timerText.color = Color.white;
            Debug.Log("Global timer started in ID_Scene");
        }
    }

    void Update()
    {
        // Only countdown if timer has started
        if (timerStarted && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            if (timerText != null)
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else if (timerStarted && timeRemaining <= 0)
        {
            if (timerText != null)
                timerText.text = "00:00";
        }
    }
}
