using UnityEngine;

public class TurnCube : MonoBehaviour
{
    /**
    The script just works in combination with a UDP Server Script, which provides the turning data.
    **/
    [SerializeField]
    private UDPServer incomingData;
    private float smooth = 10.0f;
    Quaternion offset; // Sample taken to calibrate.
    bool calibrated = false;
    public Rotator rotator;

    // Start is called before the first frame update
    void Start()
    {
        incomingData = GetComponent<UDPServer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(calibrated == true) {
            Quaternion rotation = Quaternion.Slerp(transform.localRotation, incomingData.rotationTarget,  Time.deltaTime * smooth);
            rotator.Rotate((rotation * Quaternion.Inverse(transform.rotation)).eulerAngles);
        }
    }

    public void Calibrate() {
        //see which y/90 degree the initialOffset is nearest to and set the offset to rotate to this one
        Quaternion initialOffset = Quaternion.Inverse(incomingData.rotationTarget);
        /**Vector3 euleroffset = initialOffset.eulerAngles;
        float nearest90degreeValue = Mathf.Round(euleroffset.y / 90f) * 90f;
        float offsetdifference = nearest90degreeValue - euleroffset.y;
        offset = Quaternion.Euler(0, offsetdifference, 0);**/
        offset = Quaternion.Euler(0, initialOffset.eulerAngles.y, 0);
        transform.parent.rotation = offset;
        calibrated = true;
    }
}
