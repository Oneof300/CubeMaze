using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] Rigidbody[] influencedRigidBodies;
    [SerializeField] float _influenceThreshold = 0.1f;
    [SerializeField] float _speed = 100f;

    void Update()
    {
        _speed += Input.GetKeyDown("r") ? 10f : Input.GetKeyDown("f") ? -10f : 0f;
        var rotation = new Vector3(Input.GetAxis("Vertical"), Input.GetKey("q") ? -1f : Input.GetKey("e") ? 1f : 0f, Input.GetAxis("Horizontal")) * Time.deltaTime * _speed;
        if (rotation.sqrMagnitude > _influenceThreshold) foreach (var body in influencedRigidBodies)
        {
            var localePosition = transform.InverseTransformPoint(body.position);
            var newPosition = transform.TransformPoint(Quaternion.Euler(rotation) * localePosition);
            Debug.DrawLine(body.position, newPosition, Color.red, 0f, false);
            body.AddForce((newPosition - body.position) * Time.deltaTime);
            body.AddForce(Physics.gravity * Time.deltaTime);
            body.position = newPosition;
        }
        transform.Rotate(rotation);
    }
}
