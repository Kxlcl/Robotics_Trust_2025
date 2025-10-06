using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GlobalTimer : MonoBehaviour
{
    public static GlobalTimer Instance;
    private float initialTime = 600f; // 10 minutes
    public float timeRemaining;
    private TextMeshProUGUI timerText; // Found dynamically in each scene
    public bool timerStarted = false;

    void Awake()
    {
        // Singleton pattern to prevent multiple timers
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            timeRemaining = initialTime;
            timerStarted = true; // Start timer immediately when created
            Debug.Log("GlobalTimer singleton created, marked DontDestroyOnLoad, and timer started immediately");
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
        // Immediately find timer UI in new scene
        Debug.Log($"GlobalTimer: Scene loaded: {scene.name}");
        timerText = null; // Clear old reference
        FindTimerUI();
    }
    
    public void FindTimerUI()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"GlobalTimer: Searching for timer UI in {currentScene}");

        // Try to find by GameObject name first (most reliable)
        GameObject timerObj = GameObject.Find("TimerText");
        if (timerObj == null) timerObj = GameObject.Find("Timer");
        if (timerObj == null) timerObj = GameObject.Find("TimeDisplay");

        if (timerObj != null)
        {
            timerText = timerObj.GetComponent<TextMeshProUGUI>();
            if (timerText != null)
            {
                Debug.Log($"Found timer UI: {timerObj.name} in {currentScene}");
                // Ensure timer is visible
                if (!timerText.gameObject.activeSelf)
                {
                    timerText.gameObject.SetActive(true);
                    Debug.Log($"Timer UI activated in {currentScene}");
                }
                return;
            }
        }

        // Fallback: search all TextMeshProUGUI components
        TextMeshProUGUI[] allTimerTexts = FindObjectsOfType<TextMeshProUGUI>(true);
        Debug.Log($"Fallback search: Found {allTimerTexts.Length} TextMeshProUGUI objects in scene");

        foreach (TextMeshProUGUI text in allTimerTexts)
        {
            if (text.name.ToLower().Contains("timer") || text.gameObject.name.ToLower().Contains("timer"))
            {
                timerText = text;
                Debug.Log($"Found timer UI via fallback: {text.name} on {text.gameObject.name} in {currentScene}");
                // Ensure timer is visible
                if (!timerText.gameObject.activeSelf)
                {
                    timerText.gameObject.SetActive(true);
                    Debug.Log($"Timer UI activated in {currentScene}");
                }
                return;
            }
        }

        Debug.LogWarning($"Could not find timer UI in {currentScene}. Make sure there's a GameObject named 'TimerText' or 'Timer' with a TextMeshProUGUI component.");
    }

    void Update()
    {
        // If we lost the timer reference, try to find it again
        if (timerText == null)
        {
            FindTimerUI();
        }

        // Only countdown if timer has started
        if (timerStarted && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
        else if (timerStarted && timeRemaining <= 0)
        {
            if (timerText != null)
                timerText.text = "00:00";
        }
    }
}
