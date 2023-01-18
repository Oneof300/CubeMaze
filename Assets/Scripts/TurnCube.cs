using UnityEngine;

public class TurnCube : MonoBehaviour
{
    /**
    The script just works in combination with a UDP Server Script, which provides the turning data.
    **/
    [SerializeField]
    private UDPServer incomingData;
    [SerializeField]
    private Rotator rotator;
    private float smooth = 10.0f;
    Quaternion offset; // Sample taken to calibrate.
    bool calibrated = false;

    // Start is called before the first frame update
    void Start()
    {
        incomingData = GetComponent<UDPServer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(calibrated == true) {
            var rotation = Quaternion.Slerp(transform.localRotation, incomingData.rotationTarget,  Time.deltaTime * smooth);
            rotator.Rotate((rotation * Quaternion.Inverse(transform.rotation)).eulerAngles);
        }
    }

    public void Calibrate() {
        Quaternion offset = Quaternion.Inverse(incomingData.rotationTarget);
        transform.parent.rotation = offset;
        calibrated = true;
    }
}
