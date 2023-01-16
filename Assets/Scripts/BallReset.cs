using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour
{
    Vector3 start;
    Rigidbody body;

    void Start()
    {
        start = transform.position;
        body = this.GetComponent<Rigidbody>();
    }

    public void Reset()
    {
        transform.position = start;
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }
}
