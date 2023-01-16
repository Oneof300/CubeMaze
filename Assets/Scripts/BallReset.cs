using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour
{
    Vector3 start;
    Rigidbody body;
    public Transform cube;

    void Start()
    {
        start = transform.position;
        body = this.GetComponent<Rigidbody>();
    }

    public void Reset()
    {
        transform.position = cube.rotation * start;
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }
}
