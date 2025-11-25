using UnityEngine;

/// <summary>
/// Handles player input (SPACE and optional mouse aim) and spawns the bowling ball.
/// </summary>
[RequireComponent(typeof(Transform))]
public class PlayerController : MonoBehaviour
{
    [Header("Ball Settings")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private float throwForce = 20f;

    [Header("Aiming")]
    [SerializeField] private bool aimWithMouse = true;
    [SerializeField] private float maxHorizontalOffset = 2f; // maximum left/right offset for aiming

    private GameObject currentBall;

    private void Update()
    {
        HandleAim();
        HandleInput();
    }

    /// <summary>
    /// Optionally move the spawn point left/right based on the mouse position.
    /// Only works when the player is allowed to shoot.
    /// </summary>
    private void HandleAim()
    {
        if (!aimWithMouse) return;
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.CanShoot()) return;

        // Mouse X normalized to [0..1]
        float mouseX = Input.mousePosition.x / Screen.width;
        float offset = (mouseX - 0.5f) * 2f * maxHorizontalOffset;

        Vector3 pos = ballSpawnPoint.localPosition;
        pos.x = offset;
        ballSpawnPoint.localPosition = pos;
    }

    /// <summary>
    /// Check for keyboard input (SPACE) and attempt to shoot.
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryShoot();
        }
    }

    /// <summary>
    /// Instantiate a new ball and launch it forward if the GameManager allows it.
    /// </summary>
    private void TryShoot()
    {
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.CanShoot()) return;
        if (ballPrefab == null || ballSpawnPoint == null) return;

        // Destroy previous ball instance if it still exists
        if (currentBall != null)
        {
            Destroy(currentBall);
        }

        currentBall = Instantiate(ballPrefab, ballSpawnPoint.position, ballSpawnPoint.rotation);

        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Throw forward along the player's forward direction (down the lane)
            Vector3 dir = transform.forward;
            rb.AddForce(dir * throwForce, ForceMode.Impulse);
        }

        GameManager.Instance.OnPlayerShot();
    }
}
