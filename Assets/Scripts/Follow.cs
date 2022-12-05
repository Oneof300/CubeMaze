using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] Rigidbody _target;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position, 0.9f);
        if (_target.velocity.sqrMagnitude > 1f)
            transform.LookAt(_target.position + Vector3.RotateTowards(transform.forward, _target.velocity, Mathf.PI * 0.1f, 0f));
    }
}
