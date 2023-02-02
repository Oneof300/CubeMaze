using UnityEngine;

public class Rotator : MonoBehaviour
{
    /**
    The script just works in combination with a UDP Server Script, which provides the turning data.
    **/
    [SerializeField]
    private UDPServer incomingData;
    [SerializeField]
    private RotatorCalibration calibration;
    [SerializeField]
    private float _influenceThreshold = 0.1f;
    [SerializeField]
    private float _speed = 100f;
    [SerializeField]
    private float smooth = 10.0f;
    bool calibrated = false;
    private Rigidbody _ball;
    private Rigidbody ball => _ball ? _ball : (_ball = GameObject.FindWithTag("Player")?.GetComponent<Rigidbody>());

    // Start is called before the first frame update
    void Start()
    {
        //incomingData = GetComponent<UDPServer>();
    }

    // Update is called once per frame
    void Update()
    {
        _speed += Input.GetKeyDown("r") ? 10f : Input.GetKeyDown("f") ? -10f : 0f;
        var eulerRotation = new Vector3(Input.GetAxis("Vertical"), Input.GetKey("q") ? -1f : Input.GetKey("e") ? 1f : 0f, Input.GetAxis("Horizontal")) * Time.deltaTime * _speed;
        if (eulerRotation.sqrMagnitude > 0) SetRotation(Quaternion.Slerp(transform.localRotation, transform.rotation * Quaternion.Euler(eulerRotation) * calibration.offset,  Time.deltaTime * smooth));

        if (calibrated == true) {
            Quaternion rotation = Quaternion.Slerp(transform.localRotation, incomingData.rotationTarget * calibration.offset,  Time.deltaTime * smooth);
            SetRotation(rotation);
        }
    }

    public void Calibrate() {
        //see which y/90 degree the initialOffset is nearest to and set the offset to rotate to this one
        Quaternion initialOffset = Quaternion.Inverse(incomingData.rotationTarget);
        /**Vector3 euleroffset = initialOffset.eulerAngles;
        float nearest90degreeValue = Mathf.Round(euleroffset.y / 90f) * 90f;
        float offsetdifference = nearest90degreeValue - euleroffset.y;
        offset = Quaternion.Euler(0, offsetdifference, 0);**/
        calibration.offset = Quaternion.Euler(0, initialOffset.eulerAngles.y, 0);
        calibrated = true;
    }

    public void SetRotation(Quaternion newRotation)
    {
        if (1 - Mathf.Abs(Quaternion.Dot(newRotation, transform.rotation)) > _influenceThreshold && ball)
        {
            var localePosition = transform.InverseTransformPoint(ball.position);
            var newPosition = transform.TransformPoint(newRotation * Quaternion.Inverse(transform.rotation) * localePosition);
            Debug.DrawLine(ball.position, newPosition, Color.red, 0f, false);
            ball.AddForce((newPosition - ball.position) * Time.deltaTime);
            ball.AddForce(Physics.gravity * Time.deltaTime);
            ball.position = newPosition;
        }
        transform.localRotation = newRotation;
    }
}
