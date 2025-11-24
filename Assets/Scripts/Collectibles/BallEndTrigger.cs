using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BallEndTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Constants.TAG_BALL)) return;

        GameManager.Instance?.OnBallStopped();
    }
}
