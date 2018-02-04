using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinomatron : MonoBehaviour
{

    public float speed;

    private Rigidbody rb;

    float horizontal;
    float vertical;

    void Start() {
        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate() {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);

        rb.AddForce(movement * speed * Time.deltaTime);
    }
}
