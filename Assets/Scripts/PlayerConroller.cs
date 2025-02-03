using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class MetaXRPlayerController : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public InputActionReference moveAction;
    public InputActionReference grabSwordAction;
    public InputActionReference buttonAAction;
    public InputActionReference buttonBAction;

    public float moveSpeed = 1.0f;
    public GameObject swordPrefab;
    private GameObject swordInstance;
    public Transform swordHandTransform; // The transform where the sword should be attached
    private CharacterController characterController;
    public Camera camera;
    private bool swordGrabbed = false;


    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Enable input actions
        inputActionAsset.Enable();
    }

    void Update()
    {
        // Handle movement
        Vector2 inputAxis = moveAction.action.ReadValue<Vector2>();
        Vector3 direction = new Vector3(inputAxis.x, 0, inputAxis.y);
        direction = camera.transform.TransformDirection(direction);
        direction.y = 0;
        characterController.Move(direction * Time.fixedDeltaTime * moveSpeed);

        // Handle grabbing sword
        if (!swordGrabbed && grabSwordAction.action.triggered)
        {
            GrabSword();
        }

        // Handle A and B button inputs
        if (buttonAAction.action.triggered)
        {
        }
        if (buttonBAction.action.triggered)
        {
        }
    }

    void GrabSword()
    {
        swordInstance = Instantiate(swordPrefab, swordHandTransform.position, swordHandTransform.rotation);
        swordInstance.transform.SetParent(swordHandTransform);
        swordGrabbed = true;
    }
}