using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject hand;
    Vector3 initialHandPosition;

    GameObject heldObject = null;

    IA_Basketball controls;
    float sensitivity = 9;

    bool aiming = false;

    void Start()
    {
        //setup for mouse controls
        controls = new IA_Basketball();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        controls.Enable();
        
        controls.Normal.Look.performed += Look;

        controls.Normal.Aim.performed += StartAim;
        controls.Normal.Aim.canceled += StopAim;

        controls.Normal.Fire.performed += Fire;
        //record initial hand position
        initialHandPosition = hand.transform.localPosition;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if(aiming)
        {
            //shoot the ball
        }
        else if(heldObject == null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
            
            if (Physics.Raycast(ray, out hit, 10) && hit.transform.gameObject.CompareTag("HeldObject")) 
            {
                heldObject = hit.transform.gameObject;
            }
        }
    }

    private void StopAim(InputAction.CallbackContext context)
    {
        aiming = false;
    }

    private void StartAim(InputAction.CallbackContext context)
    {
        aiming = true;
    }

    float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360+from);
        return Mathf.Min(angle, to);
    }

    private void Look(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>() * Time.deltaTime * sensitivity;
        transform.localRotation = Quaternion.Euler(
            ClampAngle(transform.localRotation.eulerAngles.x - lookInput.y, -90, 90), 
            transform.localRotation.eulerAngles.y + lookInput.x, 0);
    }

    void Update()
    {
        if(aiming)
        {
            hand.transform.localPosition = new Vector3(0, hand.transform.localPosition.y, hand.transform.localPosition.z);
        }
        else
        {
            hand.transform.localPosition = initialHandPosition;
        }

        if(heldObject != null)
        {
            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, hand.transform.position, Time.deltaTime * 20);
            heldObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void OnDestroy()
    {
        controls.Disable();
    }
}
