using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float dotSize = 4f;
    [SerializeField] private Color dotColor = Color.white;

    private Image crosshairImage;

    void Start()
    {
        // Create the crosshair dot
        GameObject dotObject = new GameObject("CrosshairDot");
        dotObject.transform.SetParent(transform, false);

        // Add and configure the Image component
        crosshairImage = dotObject.AddComponent<Image>();
        crosshairImage.color = dotColor;

        // Set the RectTransform to center and size the dot
        RectTransform rectTransform = dotObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(dotSize, dotSize);

        // Make it circular
        dotObject.AddComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
    }
}
