using UnityEngine;
using TMPro;
using System.Text;

public class ChoiceDebugConsole : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI consoleText;
    public KeyCode toggleKey = KeyCode.F1;
    public bool showOnStart = false;

    private bool isVisible = false;
    private GameObject consolePanel;

    void Start()
    {
        // If consoleText is assigned, get its parent panel
        if (consoleText != null)
        {
            consolePanel = consoleText.transform.parent.gameObject;
            consolePanel.SetActive(showOnStart);
            isVisible = showOnStart;
        }

        if (isVisible)
        {
            UpdateConsoleDisplay();
        }
    }

    void Update()
    {
        // Toggle console visibility with F1 key
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleConsole();
        }

        // Update display if visible
        if (isVisible && consoleText != null)
        {
            UpdateConsoleDisplay();
        }
    }

    void ToggleConsole()
    {
        isVisible = !isVisible;

        if (consolePanel != null)
        {
            consolePanel.SetActive(isVisible);
        }

        if (isVisible)
        {
            UpdateConsoleDisplay();
            Debug.Log("Choice Debug Console: Visible");
        }
        else
        {
            Debug.Log("Choice Debug Console: Hidden");
        }
    }

    void UpdateConsoleDisplay()
    {
        if (consoleText == null)
        {
            Debug.LogWarning("ChoiceDebugConsole: consoleText is not assigned");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== PLAYER CHOICES DEBUG ===");
        sb.AppendLine($"Press {toggleKey} to toggle this console\n");

        if (PlayerChoiceTracker.Instance == null)
        {
            sb.AppendLine("PlayerChoiceTracker Instance not found!");
        }
        else
        {
            var choices = PlayerChoiceTracker.Instance.GetAllChoices();

            if (choices.Count == 0)
            {
                sb.AppendLine("No choices recorded yet.");
            }
            else
            {
                sb.AppendLine($"Total Choices: {choices.Count}\n");

                for (int i = 0; i < choices.Count; i++)
                {
                    var choice = choices[i];
                    sb.AppendLine($"[{i + 1}] {choice.choiceId}");
                    sb.AppendLine($"    Choice: \"{choice.choiceText}\"");
                    sb.AppendLine($"    Target: {choice.targetScene}");
                    sb.AppendLine($"    Time: {choice.timestamp:F2}s");
                    sb.AppendLine();
                }
            }
        }

        // Add timer info if available
        if (GlobalTimer.Instance != null)
        {
            sb.AppendLine("--- TIMER INFO ---");
            float timeRemaining = GlobalTimer.Instance.timeRemaining;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            sb.AppendLine($"Time Remaining: {minutes:00}:{seconds:00}");
            sb.AppendLine($"Timer Started: {GlobalTimer.Instance.timerStarted}");
        }

        consoleText.text = sb.ToString();
    }

    // Public method to show console from other scripts
    public void Show()
    {
        isVisible = true;
        if (consolePanel != null)
        {
            consolePanel.SetActive(true);
        }
        UpdateConsoleDisplay();
    }

    // Public method to hide console from other scripts
    public void Hide()
    {
        isVisible = false;
        if (consolePanel != null)
        {
            consolePanel.SetActive(false);
        }
    }
}
