using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(new Vector3(Input.GetAxis("Vertical") * 0.5f, 0f, Input.GetAxis("Horizontal") * 0.5f));
    }
}
