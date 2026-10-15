using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public ParticleSystem impactDust;

    private Rigidbody rb;
    private Vector3 originalPosition;

    public float shakeDuration = 1.0f;
    public float shakeMagnitude = 0.1f;
    public float fallDelay = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Play the landing puff immediately
        impactDust.Play();
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShakeAndFall());
        }
    }

    IEnumerator ShakeAndFall()
    {
        float elapsed = 0.0f;

        // --- The Shake Phase ---
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float z = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(originalPosition.x + x, originalPosition.y, originalPosition.z + z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // --- The Fall Phase ---
        yield return new WaitForSeconds(fallDelay);

        rb.isKinematic = false; // Physics takes over
        rb.useGravity = true;   // It starts to fall

        // Optional: Destroy the stone after 3 seconds to save memory
        Destroy(gameObject, 3f);
    }
}