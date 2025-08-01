using UnityEngine;

public class CubeColorChanger : MonoBehaviour
{
    public Color newColor = new Color(1f, 0f, 0f, 1f); // Default opaque red

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Ensure color is fully opaque
            newColor.a = 1f;
            renderer.material.color = newColor;
            // Force material to be opaque (Standard shader)
            renderer.material.SetFloat("_Mode", 0); // Opaque
            renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            renderer.material.SetInt("_ZWrite", 1);
            renderer.material.DisableKeyword("_ALPHATEST_ON");
            renderer.material.DisableKeyword("_ALPHABLEND_ON");
            renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            renderer.material.renderQueue = -1;
        }
    }
}
