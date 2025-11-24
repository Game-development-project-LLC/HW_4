using UnityEngine;

[RequireComponent(typeof(Transform))]
public class PlayerController : MonoBehaviour
{
    [Header("Ball Settings")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private float throwForce = 20f;

    [Header("Aiming")]
    [SerializeField] private bool aimWithMouse = true;
    [SerializeField] private float maxHorizontalOffset = 2f; // movement left/right

    private GameObject currentBall;

    private void Update()
    {
        HandleAim();
        HandleInput();
    }

    private void HandleAim()
    {
        if (!aimWithMouse) return;
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.CanShoot()) return;

        // Move spawn point left/right with mouse on X axis (optional)
        float mouseX = Input.mousePosition.x / Screen.width; // 0..1
        float offset = (mouseX - 0.5f) * 2f * maxHorizontalOffset;

        Vector3 pos = ballSpawnPoint.localPosition;
        pos.x = offset;
        ballSpawnPoint.localPosition = pos;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.CanShoot()) return;
        if (ballPrefab == null || ballSpawnPoint == null) return;

        // Destroy previous ball if exists
        if (currentBall != null)
        {
            Destroy(currentBall);
        }

        currentBall = Instantiate(ballPrefab, ballSpawnPoint.position, ballSpawnPoint.rotation);

        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Throw forward along lane (Z axis)
            Vector3 dir = transform.forward;
            rb.AddForce(dir * throwForce, ForceMode.Impulse);
        }

        GameManager.Instance.OnPlayerShot();
    }


}
