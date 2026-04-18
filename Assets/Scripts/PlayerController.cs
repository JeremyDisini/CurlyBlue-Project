using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float maxThrowStrength = 20.0f;

    [SerializeField]
    float verticalThrowSpeed = 5.0f;

    [SerializeField]
    float slomoDuration = 0.5f;

    [SerializeField]
    float throwStrengthFovInfluence = 20.0f;

    [SerializeField]
    GameObject hand;

    float baseFov;
    Vector3 initialHandPosition;

    GameObject heldObject = null;
    public GameObject hoveredObject = null;

    IA_Basketball controls;
    float sensitivity = 0.1f;

    float throwStrength = 0.0f;
    float timeToMaxStrength = 2.0f;

    bool aiming = false;

    void Start()
    {
        //controls setup
        controls = new IA_Basketball();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        controls.Enable();
        controls.Normal.Look.performed += Look;
        controls.Normal.Aim.performed += StartAim;
        controls.Normal.Aim.canceled += StopAim;
        controls.Normal.Fire.performed += Fire;
        
        initialHandPosition = hand.transform.localPosition;

        baseFov = Camera.main.fieldOfView;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        //if holding an object, then fire it
        if(heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            heldObject = null;
            rb.isKinematic = false;
            rb.linearVelocity = transform.forward * (maxThrowStrength * throwStrength) + transform.up * verticalThrowSpeed;
            rb.angularVelocity = Vector3.zero;

            //trigger a slow motion effect when firing
            StartCoroutine(SlowMo());
        }
        //otherwise, if hovering over an object, then pick it up
        else if(hoveredObject != null)
        {
            heldObject = hoveredObject;
        }
    }

    //coroutine for a slow motion effect
    IEnumerator SlowMo()
    {
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = Time.timeScale * Time.fixedUnscaledDeltaTime;
        yield return new WaitForSecondsRealtime(slomoDuration);
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale * Time.fixedUnscaledDeltaTime;
    }

    //mouselook function, called whenever any amount of mouse input is detected
    private void Look(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>() * sensitivity;
        transform.localRotation = Quaternion.Euler(
            Utils.ClampAngle(transform.localRotation.eulerAngles.x - lookInput.y, -90, 90), 
            transform.localRotation.eulerAngles.y + lookInput.x, 0);
    }

    void Update()
    {
        //handle the hand's position depending on whether the player is aiming or not
        if(aiming && heldObject != null)
        {
            hand.transform.localPosition = new Vector3(0, hand.transform.localPosition.y, hand.transform.localPosition.z);
            
            //steadily increase throw strength while aiming
            throwStrength += 1.0f/timeToMaxStrength * Time.deltaTime;
            throwStrength = Mathf.Clamp(throwStrength, 0, 1);
        }
        else
        {
            hand.transform.localPosition = initialHandPosition;
            throwStrength = Mathf.Lerp(throwStrength, 0, Time.deltaTime * 10);
        }

        // change camera FOV based on throw strength
        Camera.main.fieldOfView = baseFov - (throwStrengthFovInfluence * Utils.EaseOut(throwStrength));

        //teleport held object to 
        if(heldObject != null)
        {
            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, hand.transform.position, Time.deltaTime * 20);
            heldObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        // fire a ray to detect possible pickups
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));

        //if ray hits something, register as a hovered object
        if (Physics.Raycast(ray, out hit, 10) && hit.transform.gameObject.CompareTag("HeldObject")) 
        {
            hoveredObject = hit.transform.gameObject;
        }
        else
        {
            hoveredObject = null;
        }
    }

    void OnDestroy()
    {
        controls.Disable();
    }


    private void StopAim(InputAction.CallbackContext context)
    {
        aiming = false;
    }

    private void StartAim(InputAction.CallbackContext context)
    {
        aiming = true;
    }
}
