using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] Rigidbody _target;
    [SerializeField] Transform _camera;

    void Update()
    {
        _target.AddForce(_camera.right * Input.GetAxis("Horizontal") + _camera.forward * Input.GetAxis("Vertical"));
    }
}
