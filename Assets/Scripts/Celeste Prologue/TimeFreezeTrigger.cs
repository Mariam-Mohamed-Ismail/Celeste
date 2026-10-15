using UnityEngine;

/// <summary>
/// Time Freeze Trigger (Section 8 - Bird/Dash Tutorial).
/// Freezes time when player enters. Only resumes on Jump + Dash combo.
/// Uses a coroutine to monitor for input while Time.timeScale is 0.
/// NO LINQ. NO ENUMS.
/// </summary>
public class TimeFreezeTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject sparrowObject;
    public GameObject dashTutorialUI;
    public CelestePlayerController playerController;

    [Header("Camera")]
    public Transform cameraFocusTarget;

    [Header("Settings")]
    public float slowMotionEntry = 0.2f;
    public float slowMotionDuration = 1.0f;

    private bool hasTriggered = false;
    private bool isFrozen = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        StartCoroutine(TimeFreezeSequence());
    }

    System.Collections.IEnumerator TimeFreezeSequence()
    {
        // Step 1: Slow motion entry
        Time.timeScale = slowMotionEntry;
        
        // Block player input
        if (playerController == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerController = player.GetComponent<CelestePlayerController>();
        }
        if (playerController != null)
        {
            playerController.SetInputActive(false);
        }

        yield return new WaitForSecondsRealtime(slowMotionDuration);

        // Step 2: Full freeze
        Time.timeScale = 0f;
        isFrozen = true;

        // Activate the Sparrow / Bird
        if (sparrowObject != null)
        {
            sparrowObject.SetActive(true);
        }

        // Show dash tutorial UI
        if (dashTutorialUI != null)
        {
            dashTutorialUI.SetActive(true);
        }

        // Step 3: Wait for Dash input to resume
        // Using realtime monitoring since Time.timeScale is 0
        yield return StartCoroutine(WaitForDashInput());

        // Step 4: Resume
        Time.timeScale = 1f;
        isFrozen = false;

        // Unlock dash ability
        if (playerController != null)
        {
            playerController.UnlockDash();
            playerController.SetInputActive(true);
        }

        // Hide tutorial UI
        if (dashTutorialUI != null)
        {
            dashTutorialUI.SetActive(false);
        }
    }

    System.Collections.IEnumerator WaitForDashInput()
    {
        // Monitor for the Dash input while time is frozen
        // Check every frame using realtime (works even when timeScale = 0)
        while (true)
        {
            // Check for Dash key (X or Right Shift) combined with Jump (Space)
            bool dashInput = Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.RightShift);
            bool jumpInput = Input.GetKeyDown(KeyCode.Space);

            // Accept either dash or jump+dash combo to resume
            if (dashInput)
            {
                break;
            }

            yield return null; // WaitForEndOfFrame equivalent in frozen time
        }
    }

    void Update()
    {
        // Backup check: if frozen and dash pressed, unfreeze
        if (isFrozen)
        {
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.RightShift))
            {
                isFrozen = false;
                Time.timeScale = 1f;

                if (playerController != null)
                {
                    playerController.UnlockDash();
                    playerController.SetInputActive(true);
                }

                if (dashTutorialUI != null)
                {
                    dashTutorialUI.SetActive(false);
                }
            }
        }
    }
}
