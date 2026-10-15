using UnityEngine;

/// <summary>
/// Crumbling Platform (Section 6 - Trembling Rocks).
/// On collision: shake -> after 1.5s disable collider + add Rigidbody -> fall.
/// NO LINQ. NO ENUMS.
/// </summary>
public class CrumblingPlatform : MonoBehaviour
{
    [Header("Settings")]
    public float shakeTime = 1.5f;
    public float shakeIntensity = 0.03f;
    public float respawnDelay = 5f;
    public bool canRespawn = false;

    private bool isShaking = false;
    private bool hasFallen = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Collider platformCollider;
    private MeshRenderer meshRenderer;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        platformCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        
        // Find collider in children if not on root
        if (platformCollider == null)
        {
            platformCollider = GetComponentInChildren<Collider>();
        }
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasFallen || isShaking) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(CrumbleSequence());
        }
    }

    System.Collections.IEnumerator CrumbleSequence()
    {
        isShaking = true;

        // Shake for shakeTime seconds
        float timer = 0f;
        while (timer < shakeTime)
        {
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetZ = Random.Range(-shakeIntensity, shakeIntensity);
            transform.position = originalPosition + new Vector3(offsetX, offsetY, offsetZ);
            timer += Time.deltaTime;
            yield return null;
        }

        // Fall
        hasFallen = true;
        isShaking = false;

        // Disable collider
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }

        // Add Rigidbody to make it fall
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 10f;
        rb.useGravity = true;

        // Wait then either respawn or destroy
        yield return new WaitForSeconds(respawnDelay);

        if (canRespawn)
        {
            // Remove rigidbody, reset position
            Destroy(rb);
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            if (platformCollider != null) platformCollider.enabled = true;
            hasFallen = false;
        }
        else
        {
            // Disable renderer but keep object (could fade out)
            if (meshRenderer != null) meshRenderer.enabled = false;
        }
    }
}
