using UnityEngine;

/// <summary>
/// Base behaviour for a bowling pin.
/// Tracks its initial orientation and position, and can tell whether the pin has fallen.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class EnemyBase : MonoBehaviour
{
    [Header("Pin Settings")]
    [Tooltip("How many degrees away from the original 'up' direction the pin must tilt to be considered fallen.")]
    [SerializeField] private float fallAngleThreshold = 15f;

    /// <summary>True if this pin has tilted beyond the threshold and is considered down.</summary>
    public bool IsDown { get; private set; } = false;

    /// <summary>
    /// True if this pin has already been counted and removed from the lane
    /// during this evaluation step.
    /// </summary>
    public bool RemovedThisFrame { get; private set; } = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialUp;     // "Up" direction when the pin is standing.
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Store the starting pose so we can detect tilt and reset later.
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialUp = transform.up;
    }

    private void Update()
    {
        // Once a pin is marked as down, we do not need to re-check it.
        if (IsDown) return;

        // Measure how much the pin has tilted away from its original up direction.
        float angle = Vector3.Angle(transform.up, initialUp);

        if (angle > fallAngleThreshold)
        {
            IsDown = true;
        }
    }

    /// <summary>
    /// Reset the pin back to its original position/orientation and clear physics.
    /// </summary>
    public void ResetPin()
    {
        IsDown = false;
        RemovedThisFrame = false;

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Reset any remaining motion from the rigidbody.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide this pin from the lane after it has been counted as fallen.
    /// </summary>
    public void RemoveFromLane()
    {
        RemovedThisFrame = true;
        gameObject.SetActive(false);
    }
}
