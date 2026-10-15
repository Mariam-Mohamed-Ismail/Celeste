using UnityEngine;

/// <summary>
/// Death Zone - respawns player at the last checkpoint.
/// NO LINQ. NO ENUMS.
/// </summary>
public class DeathZone : MonoBehaviour
{
    public Transform respawnPoint;
    
    private static Vector3 lastCheckpoint = Vector3.zero;
    private static bool checkpointSet = false;

    public static void SetCheckpoint(Vector3 position)
    {
        lastCheckpoint = position;
        checkpointSet = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Respawn player
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            
            if (checkpointSet)
            {
                other.transform.position = lastCheckpoint + Vector3.up * 2f;
            }
            else if (respawnPoint != null)
            {
                other.transform.position = respawnPoint.position;
            }
            else
            {
                other.transform.position = new Vector3(97f, 11f, 0f); // Default spawn
            }
            
            cc.enabled = true;
        }
    }
}
