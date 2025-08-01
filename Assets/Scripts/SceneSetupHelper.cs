using UnityEngine;

public class SceneSetupHelper : MonoBehaviour
{
    public GameObject cube;
    public Camera mainCamera;
    public Light sceneLight;

    void Start()
    {
        // Create cube if not assigned
        if (cube == null)
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.zero;
        }
        else
        {
            cube.transform.position = Vector3.zero;
        }

        // Position camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0, 0, -5);
            mainCamera.transform.rotation = Quaternion.identity;
            mainCamera.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        }

        // Add directional light if not assigned
        if (sceneLight == null)
        {
            sceneLight = new GameObject("Directional Light").AddComponent<Light>();
            sceneLight.type = LightType.Directional;
            sceneLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
    }
}
