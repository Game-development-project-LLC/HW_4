
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private bool destroyOnCollect = true;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag(Constants.TAG_BALL)) return;

        collected = true;

        Debug.Log($"Collected coin +{scoreValue}");

        if (destroyOnCollect)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
