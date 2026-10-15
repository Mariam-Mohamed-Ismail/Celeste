using UnityEngine;

/// <summary>
/// Falling Snow Block (Section 3).
/// Drops via Rigidbody after the player exits the trigger area.
/// NO LINQ. NO ENUMS.
/// </summary>
public class FallingBlock : MonoBehaviour
{
    [Header("Settings")]
    public float fallDelay = 0.5f;
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.3f;

    private bool playerInside = false;
    private bool hasTriggered = false;
    private Vector3 originalPosition;
    private Rigidbody rb;

    void Start()
    {
        originalPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (hasTriggered) return;
        if (other.CompareTag("Player") && playerInside)
        {
            hasTriggered = true;
            StartCoroutine(FallSequence());
        }
    }

    System.Collections.IEnumerator FallSequence()
    {
        // Shake warning
        float shakeTimer = 0f;
        while (shakeTimer < shakeDuration)
        {
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
            transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);
            shakeTimer += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;

        // Wait before falling
        yield return new WaitForSeconds(fallDelay);

        // Enable gravity and fall
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.mass = 50f;

        // Destroy after falling far enough
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
