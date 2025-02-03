using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMovementController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    private CharacterController characterController;
    public Camera camera;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get keyboard input
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

        // Create a movement direction based on the input
        Vector3 direction = new Vector3(moveX, 0, moveZ);
        direction = camera.transform.TransformDirection(direction);
        direction.y = 0;

        // Move the character
        characterController.Move(direction * Time.fixedDeltaTime * moveSpeed);
    }
}
