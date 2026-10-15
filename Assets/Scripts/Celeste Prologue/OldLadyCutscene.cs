using UnityEngine;

/// <summary>
/// Old Lady Cutscene Trigger (Section 4-5).
/// Slows time, blocks input, focuses camera, shows dialogue.
/// NO LINQ. NO ENUMS.
/// </summary>
public class OldLadyCutscene : MonoBehaviour
{
    [Header("References")]
    public CelestePlayerController playerController;
    public Transform oldLadyTransform;
    public Transform cameraLookAtTarget;

    [Header("Dialogue Lines")]
    public string[] dialogueLines = new string[]
    {
        "Oh my... Another one.",
        "You should turn back, dear.",
        "This mountain doesn't want you here.",
        "No one can make it to the top.",
        "And yet... here you are.",
        "Be careful up there."
    };

    [Header("Settings")]
    public float slowMotionScale = 0.2f;
    public float dialogueDuration = 3f;
    public float lineDelay = 2.5f;

    private bool hasTriggered = false;
    private bool showingDialogue = false;
    private string currentLine = "";
    private int currentLineIndex = 0;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        
        if (playerController == null)
        {
            playerController = other.GetComponent<CelestePlayerController>();
        }
        
        StartCoroutine(CutsceneSequence());
    }

    System.Collections.IEnumerator CutsceneSequence()
    {
        // Block player input
        if (playerController != null)
        {
            playerController.SetInputActive(false);
        }

        // Slow motion entry
        Time.timeScale = slowMotionScale;
        yield return new WaitForSecondsRealtime(0.5f);

        // Show dialogue
        showingDialogue = true;
        
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            currentLineIndex = i;
            currentLine = dialogueLines[i];
            yield return new WaitForSecondsRealtime(lineDelay);
        }

        // End dialogue
        showingDialogue = false;
        currentLine = "";

        // Resume time
        Time.timeScale = 1f;

        // Re-enable player input
        if (playerController != null)
        {
            playerController.SetInputActive(true);
        }
    }

    void OnGUI()
    {
        if (!showingDialogue) return;

        // Simple dialogue display using OnGUI (as per rules - no Dialogue System installed)
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 24;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        style.wordWrap = true;
        style.padding = new RectOffset(20, 20, 15, 15);

        float boxWidth = 600f;
        float boxHeight = 80f;
        float x = (Screen.width - boxWidth) * 0.5f;
        float y = Screen.height - boxHeight - 80f;

        // Background
        GUI.backgroundColor = new Color(0f, 0f, 0f, 0.8f);
        GUI.Box(new Rect(x, y, boxWidth, boxHeight), currentLine, style);

        // Speaker name
        GUIStyle nameStyle = new GUIStyle(GUI.skin.label);
        nameStyle.fontSize = 16;
        nameStyle.normal.textColor = new Color(0.8f, 0.6f, 1f, 1f);
        nameStyle.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(x + 10f, y - 25f, 200f, 25f), "Old Woman", nameStyle);
    }
}
