using UnityEngine;

/// <summary>
/// Checkpoint trigger. Sets the player's respawn point.
/// NO LINQ. NO ENUMS.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        DeathZone.SetCheckpoint(transform.position);
    }
}
