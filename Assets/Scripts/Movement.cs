using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        
        if (controller.isGrounded)
            velocity.y = 0f;
        
        velocity.y += gravity * Time.deltaTime;
        move.y = velocity.y;
        
        controller.Move(move * speed * Time.deltaTime);
    }
}
