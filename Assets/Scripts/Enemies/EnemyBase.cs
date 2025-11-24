using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class EnemyBase : MonoBehaviour
{
    [Header("Pin Settings")]
    [SerializeField] private float fallAngleThreshold = 15f; 

    public bool IsDown { get; private set; } = false;
    public bool RemovedThisFrame { get; private set; } = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialUp; 
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialUp = transform.up;
    }

    private void Update()
    {
        if (IsDown) return;

        // 
        float angle = Vector3.Angle(transform.up, initialUp);

        if (angle > fallAngleThreshold)
        {
            IsDown = true;
        }
    }

    public void ResetPin()
    {
        IsDown = false;
        RemovedThisFrame = false;

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        gameObject.SetActive(true);
    }

    public void RemoveFromLane()
    {
        RemovedThisFrame = true;
        gameObject.SetActive(false);
    }
}
