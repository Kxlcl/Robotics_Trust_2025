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
        CheckSceneAndStartTimer();
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
