using UnityEngine;

/// <summary>
/// Simple collectible that reacts when the ball enters its trigger.
/// Can optionally be destroyed or just deactivated after collection.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CollectibleItem : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private bool destroyOnCollect = true;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        // Ignore if already collected once
        if (collected) return;

        // Only react to the bowling ball
        if (!other.CompareTag(Constants.TAG_BALL)) return;

        collected = true;

        // In a full game this would be hooked into a score system
        Debug.Log($"Collected item +{scoreValue}");

        if (destroyOnCollect)
        {
            Destroy(gameObject);
        }
        else
        {
            // Keep the object in the scene but hide it
            gameObject.SetActive(false);
        }
    }
}
