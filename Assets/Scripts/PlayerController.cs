using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject hand;
    Vector3 initialHandPosition;

    GameObject heldObject = null;

    IA_Basketball controls;
    float sensitivity = 0.1f;

    float throwStrength = 0.0f;
    float timeToMaxStrength = 2.0f;

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
        if(heldObject != null && aiming)
        {
            //shoot the ball
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            heldObject = null;
            rb.isKinematic = false;
            rb.linearVelocity = transform.forward * (20 * throwStrength) + transform.up * 5;
            rb.angularVelocity = Vector3.zero;
            StartCoroutine(SlowMo());
        }
        else if(heldObject == null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
            
            if (Physics.Raycast(ray, out hit, 1000) && hit.transform.gameObject.CompareTag("HeldObject")) 
            {
                heldObject = hit.transform.gameObject;
            }
        }
    }

    IEnumerator SlowMo()
    {
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = Time.timeScale * Time.fixedUnscaledDeltaTime;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale * Time.fixedUnscaledDeltaTime;
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
        Vector2 lookInput = context.ReadValue<Vector2>() * sensitivity;
        transform.localRotation = Quaternion.Euler(
            ClampAngle(transform.localRotation.eulerAngles.x - lookInput.y, -90, 90), 
            transform.localRotation.eulerAngles.y + lookInput.x, 0);
    }

    float EaseOut(float x)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
    }

    void Update()
    {
        if(aiming && heldObject != null)
        {
            hand.transform.localPosition = new Vector3(0, hand.transform.localPosition.y, hand.transform.localPosition.z);
            throwStrength += 1.0f/timeToMaxStrength * Time.deltaTime;
            throwStrength = Mathf.Clamp(throwStrength, 0, 1);
        }
        else
        {
            hand.transform.localPosition = initialHandPosition;
            throwStrength = Mathf.Lerp(throwStrength, 0, Time.deltaTime * 10);
        }

        if(heldObject != null)
        {
            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, hand.transform.position, Time.deltaTime * 20);
            heldObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        Camera.main.fieldOfView = 80.0f - (20.0f * EaseOut(throwStrength));
    }

    void OnDestroy()
    {
        controls.Disable();
    }
}
