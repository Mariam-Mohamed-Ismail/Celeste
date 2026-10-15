using UnityEngine;
using UnityEngine.SceneManagement; // Required for switching scenes

public class CelesteEndTrigger : MonoBehaviour
{
    [Header("Settings")]
    public string OutroScene; // Type the exact name of your next scene here
    public float delayBeforeLoad = 0.1f; // Optional slight pause for feel

    private bool cutsceneStarted = false;

    void OnTriggerEnter(Collider other)
    {
        if (cutsceneStarted) return;

        if (other.CompareTag("Player"))
        {
            cutsceneStarted = true;
            Debug.Log("End Trigger Hit! Loading: " + OutroScene);

            // Start the transition
            Invoke("LoadNextLevel", delayBeforeLoad);
        }
    }

    void LoadNextLevel()
    {
        // Make sure the scene name matches exactly what is in your project folders
        if (!string.IsNullOrEmpty(OutroScene))
        {
            SceneManager.LoadScene(OutroScene);
        }
        else
        {
            Debug.LogError("Next Scene Name is empty in the Inspector!");
        }
    }
}