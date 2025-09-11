using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GlobalTimer : MonoBehaviour
{
    // Removed static instance for persistence
    private float initialTime = 600f; // 10 minutes
    public float timeRemaining;
    public TextMeshProUGUI timerText; // Assign in Inspector
    private bool timerStarted = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        timeRemaining = initialTime;
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
        
        if (currentScene == "ID_Scene" && !timerStarted)
        {
            StartTimer();
        }
        else if (currentScene != "ID_Scene")
        {
            // Hide timer in other scenes
            if (timerText != null)
            {
                timerText.gameObject.SetActive(false);
            }
        }
        else if (currentScene == "ID_Scene" && timerStarted)
        {
            // Show timer if already started and we're in ID_Scene
            if (timerText != null)
            {
                timerText.gameObject.SetActive(true);
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
