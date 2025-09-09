using UnityEngine;

public class MovementDebug : MonoBehaviour
{
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (horizontal != 0 || vertical != 0)
        {
            Debug.Log($"Input detected - Horizontal: {horizontal}, Vertical: {vertical}");
        }
        
        if (Input.GetKey(KeyCode.W))
            Debug.Log("W key pressed");
        if (Input.GetKey(KeyCode.A))
            Debug.Log("A key pressed");
        if (Input.GetKey(KeyCode.S))
            Debug.Log("S key pressed");
        if (Input.GetKey(KeyCode.D))
            Debug.Log("D key pressed");
    }
}