using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider bx;
    private bool disableRotation;
    public float destroyTime = 10f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bx = GetComponent<BoxCollider>();
        
        Destroy(this.gameObject, destroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        disableRotation = true;
        rb.isKinematic = true;
        bx.isTrigger = true;

    }

    private void Update()
    {
        if (!disableRotation)
        {
           transform.rotation = Quaternion.LookRotation(rb.velocity); 
        }
    }
}
