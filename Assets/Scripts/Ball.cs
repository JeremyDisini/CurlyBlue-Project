using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField]
    Vector3 initialPosition;

    [SerializeField]
    Rigidbody rb;

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("BallReset"))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = initialPosition;
        }
    }   
}
