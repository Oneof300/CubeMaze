using UnityEngine;

public class RotationSound : MonoBehaviour
{
    public AudioClip clip;
    private Quaternion previousRotation;
    private float rotationDelta;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        previousRotation = transform.localRotation;
        audioSource.clip = clip;
        audioSource.Play();
    }

    void Update()
    {
        rotationDelta = Quaternion.Angle(transform.localRotation, previousRotation);
        if(1 + rotationDelta > 3) {
            audioSource.pitch = 3;
        } else {
            audioSource.pitch = 1 + rotationDelta;
        }
        previousRotation = transform.localRotation;
    }
}