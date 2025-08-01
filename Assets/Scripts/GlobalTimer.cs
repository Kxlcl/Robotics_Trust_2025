using UnityEngine;
using TMPro;

public class GlobalTimer : MonoBehaviour
{
    // Removed static instance for persistence
    private float initialTime = 600f; // 10 minutes
    public float timeRemaining;
    public TextMeshProUGUI timerText; // Assign in Inspector

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        timeRemaining = initialTime;
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            if (timerText != null)
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            if (timerText != null)
                timerText.text = "00:00";
        }
    }
}
