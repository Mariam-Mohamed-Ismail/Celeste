using UnityEngine;
using System.Collections;

public class TremblingPlatform : MonoBehaviour
{
    public float shakeTime = 1.5f;
    public float shakeAmount = 0.05f;
    private bool isShaking = false;
    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isShaking)
        {
            StartCoroutine(ShakeThenFall());
        }
    }

    IEnumerator ShakeThenFall()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeTime)
        {
            transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Fall
        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false; // Disable collider per rule 3

        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
