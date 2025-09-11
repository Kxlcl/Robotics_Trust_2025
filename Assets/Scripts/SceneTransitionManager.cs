using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    
    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;
    private GameObject canvasGO;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void CreateFadeOverlay()
    {
        // Create Canvas
        canvasGO = new GameObject("Fade Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000; // Make sure it's on top
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create Image
        GameObject imageGO = new GameObject("Fade Image");
        imageGO.transform.SetParent(canvasGO.transform);
        
        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = Color.black;
        
        // Set to full screen
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        DontDestroyOnLoad(canvasGO);
    }
    
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(FadeToScene(sceneName));
    }
    
    IEnumerator FadeToScene(string sceneName)
    {
        // Create fade overlay
        CreateFadeOverlay();
        
        // Fade out (to black)
        yield return StartCoroutine(FadeOut());
        
        // Load new scene
        SceneManager.LoadScene(sceneName);
        
        // Wait a frame for scene to load
        yield return null;
        
        // Fade in (from black) - check if fadeImage still exists
        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeIn());
        }
        
        // Destroy fade overlay after transition
        if (canvasGO != null)
        {
            Destroy(canvasGO);
            fadeImage = null;
            canvasGO = null;
        }
    }
    
    IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;
        
        float timer = 0f;
        
        while (timer < fadeDuration && fadeImage != null)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            Color currentColor = fadeImage.color;
            currentColor.a = alpha;
            fadeImage.color = currentColor;
            yield return null;
        }
        
        // Ensure fully opaque
        if (fadeImage != null)
        {
            Color finalColor = fadeImage.color;
            finalColor.a = 1f;
            fadeImage.color = finalColor;
        }
    }
    
    IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;
        
        float timer = 0f;
        
        while (timer < fadeDuration && fadeImage != null)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            Color currentColor = fadeImage.color;
            currentColor.a = alpha;
            fadeImage.color = currentColor;
            yield return null;
        }
        
        // Ensure fully transparent
        if (fadeImage != null)
        {
            Color finalColor = fadeImage.color;
            finalColor.a = 0f;
            fadeImage.color = finalColor;
        }
    }
}