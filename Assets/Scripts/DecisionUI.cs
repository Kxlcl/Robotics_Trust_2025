using UnityEngine;
using UnityEngine.UI;

public class DecisionUI : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject decisionPanel;
    public Text questionText;
    public Transform buttonContainer;
    public Button buttonPrefab;
    public Button closeButton;

    [Header("Animation")]
    public float fadeInDuration = 0.5f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private CanvasGroup canvasGroup;
    private DecisionActionManager currentDecisionManager;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (closeButton != null)
            closeButton.onClick.AddListener(HideDecision);

        HideDecision();
    }

    public void ShowDecision(DecisionActionManager decisionManager)
    {
        currentDecisionManager = decisionManager;
        
        if (decisionPanel != null)
            decisionPanel.SetActive(true);

        // Unlock cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause player movement
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null)
            player.enabled = false;

        StartCoroutine(FadeIn());
    }

    public void HideDecision()
    {
        if (decisionPanel != null)
            decisionPanel.SetActive(false);

        // Lock cursor back for FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Resume player movement
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null)
            player.enabled = true;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    System.Collections.IEnumerator FadeIn()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeInDuration;
            canvasGroup.alpha = fadeInCurve.Evaluate(progress);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    public void OnChoiceMade()
    {
        HideDecision();
    }
}