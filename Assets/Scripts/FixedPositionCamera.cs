using UnityEngine;

public class FixedPositionCamera : MonoBehaviour
{
    public float sensitivity = 2f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Keep camera at its starting position
        transform.position = startPosition;

        // Mouse look (cursor is not locked, so UI remains interactive)
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(Vector3.up, mouseX, Space.World);
        transform.Rotate(Vector3.right, -mouseY, Space.Self);
    }
}
