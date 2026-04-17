using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    IA_Basketball controls;

    public float xRotation = 0;
    float sensitivity = 6.0f;
    Vector2 lookInput;
    void Start()
    {
        controls = new IA_Basketball();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        controls.Normal.Look.performed += Look;
        controls.Enable();
    }

    private void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>() * Time.deltaTime * sensitivity;
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x - lookInput.y, transform.localRotation.eulerAngles.y + lookInput.x, 0);
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        controls.Disable();
    }
}
