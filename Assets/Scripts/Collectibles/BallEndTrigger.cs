using UnityEngine;

/// <summary>
/// Trigger placed at the end of the lane.
/// When the ball enters this trigger, the GameManager is notified that the roll is over.
/// </summary>
[RequireComponent(typeof(Collider))]
public class BallEndTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Only react to the bowling ball
        if (!other.CompareTag(Constants.TAG_BALL)) return;

        // Inform the GameManager that the ball has finished its roll
        GameManager.Instance?.OnBallStopped();
    }
}
