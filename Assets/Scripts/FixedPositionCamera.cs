using UnityEngine;

public class FixedPositionCamera : MonoBehaviour
{
    public float sensitivity = 2f;
    private Vector3 startPosition;
    private bool gameStarted = false;

    void Start()
    {
        startPosition = transform.position;
        // Don't set cursor state here - let PlayerController handle it
        Cursor.visible = true;
    }

    void Update()
    {
        // Keep camera at its starting position
        transform.position = startPosition;

        // Only allow mouse look after game has started
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null && playerController.gameStarted)
        {
            gameStarted = true;
        }

        if (gameStarted)
        {
            // Mouse look 
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            transform.Rotate(Vector3.up, mouseX, Space.World);
            transform.Rotate(Vector3.right, -mouseY, Space.Self);
        }
    }
}
