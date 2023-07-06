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
    [SerializeField]
    private float bounce = 1f;
    [SerializeField]
    private float drag = 0.1f;
    private GameObject _ball;
    private GameObject ball => _ball ? _ball : (_ball = GameObject.FindWithTag("Player"));
    private Vector3 previousBallPosition;
    private Quaternion previousRotation;
    private int ballCollisionIterations;

    void Start()
    {
        transform.parent.localRotation = calibration.offset;
        transform.rotation = Quaternion.identity;

        previousBallPosition = ball.transform.position;
        previousRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        _speed += Input.GetKeyDown("r") ? 10f : Input.GetKeyDown("f") ? -10f : 0f;
        var eulerRotation = new Vector3(Input.GetAxis("Vertical"), Input.GetKey("q") ? -1f : Input.GetKey("e") ? 1f : 0f, Input.GetAxis("Horizontal")) * Time.deltaTime * _speed;
        if (eulerRotation.sqrMagnitude > 0) SetRotation(Quaternion.Slerp(transform.localRotation, transform.rotation * Quaternion.Euler(eulerRotation),  Time.deltaTime * smooth));

        if (calibration.calibrated == true)
        {
            Quaternion rotation = Quaternion.Slerp(transform.localRotation, incomingData.rotationTarget,  Time.fixedDeltaTime * smooth);
            SetRotation(rotation);
        }

        if (!ball.activeSelf) return;
        ballCollisionIterations = 0;
        HandleBallCollision();

        previousBallPosition = ball.transform.position;
        previousRotation = transform.rotation;
    }

    private void HandleBallCollision()
    {
        var deltaRotation = transform.rotation * Quaternion.Inverse(previousRotation);
        var relativePreviousBallPosition = deltaRotation * previousBallPosition;
        if (!Physics.Linecast(relativePreviousBallPosition, ball.transform.position, out var hit, 1))
            return;

        var ballRadius = ball.GetComponent<SphereCollider>().radius * 0.5f;
        var reflectionPoint = hit.point + ballRadius * hit.normal;
        // var ballDelta = ball.transform.position - relativePreviousBallPosition;
        // var remainingTravelDistance = ballDelta.magnitude - Vector3.Distance(relativePreviousBallPosition, reflectionPoint);
        var ballBody = ball.GetComponent<Rigidbody>();
        ballBody.position = reflectionPoint;
        // ballBody.position = reflectionPoint + remainingTravelDistance * ballBody.velocity.normalized;
        // ballBody.velocity = Vector3.Reflect(ballDelta / Time.fixedDeltaTime, hit.normal) * bounce;

        Debug.DrawRay(hit.point, hit.normal, Color.white, 0.5f);
        // ballVelocity = Vector3.Reflect(ballDelta / Time.fixedDeltaTime, hit.normal) * bounce;
        // ball.transform.position = hit.point + Vector3.Reflect(relativePreviousBallPosition - hit.point, hit.normal) + hit.normal * 0.01f;
        
        if (++ballCollisionIterations >= 3) return;
        HandleBallCollision();
    }

    public void Calibrate()
    {
        Quaternion initialOffset = Quaternion.Inverse(incomingData.rotationTarget);
        calibration.offset = Quaternion.Euler(0, initialOffset.eulerAngles.y, 0);
        transform.parent.localRotation = calibration.offset;
        calibration.calibrated = true;
    }

    public void SetRotation(Quaternion newRotation)
    {
        // if (ball && Quaternion.Angle(transform.localRotation, newRotation) > _influenceThreshold)
        // {
        //     var localePosition = transform.InverseTransformPoint(ball.position);
        //     var newPosition = transform.TransformPoint(Quaternion.Inverse(transform.rotation) * newRotation * localePosition);
        //     Debug.DrawLine(ball.position, newPosition, Color.red, 0f, false);
        //     ball.AddForce((newPosition - ball.position) * Time.deltaTime);
        //     ball.AddForce(Physics.gravity * Time.deltaTime);
        //     ball.position = newPosition;
        // }
        transform.localRotation = newRotation;
    }
}
