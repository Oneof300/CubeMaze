using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float _influenceThreshold = 0.1f;
    [SerializeField] float _speed = 100f;

    Rigidbody _ball; Rigidbody ball => _ball ? _ball : (_ball = GameObject.FindWithTag("Player").GetComponent<Rigidbody>());

    void Update()
    {
        _speed += Input.GetKeyDown("r") ? 10f : Input.GetKeyDown("f") ? -10f : 0f;
        var rotation = new Vector3(Input.GetAxis("Vertical"), Input.GetKey("q") ? -1f : Input.GetKey("e") ? 1f : 0f, Input.GetAxis("Horizontal")) * Time.deltaTime * _speed;
        Rotate(rotation);
    }

    public void Rotate(Vector3 rotation)
    {
        if (rotation.sqrMagnitude > _influenceThreshold)
        {
            var localePosition = transform.InverseTransformPoint(ball.position);
            var newPosition = transform.TransformPoint(Quaternion.Euler(rotation) * localePosition);
            Debug.DrawLine(ball.position, newPosition, Color.red, 0f, false);
            ball.AddForce((newPosition - ball.position) * Time.deltaTime);
            ball.AddForce(Physics.gravity * Time.deltaTime);
            ball.position = newPosition;
        }
        transform.Rotate(rotation);
    }
}
