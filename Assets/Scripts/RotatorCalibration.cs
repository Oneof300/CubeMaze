using UnityEngine;

[CreateAssetMenu]
public class RotatorCalibration : ScriptableObject
{
    public Quaternion offset;
    public bool calibrated = false;
}
