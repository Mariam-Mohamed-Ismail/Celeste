using UnityEngine;
using Unity.Cinemachine;

public class StartOldLadyCutscene : MonoBehaviour
{
    public CinemachineCamera gameplayCam;
    public CinemachineCamera cutsceneCam;
    public CelestePlayerController player;
    public CelesteDialogueSystem dialogueSystem;
    
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            
            // Camera switch
            if (gameplayCam != null) gameplayCam.Priority = 10;
            if (cutsceneCam != null) cutsceneCam.Priority = 20;

            // Stop player movement
            if (player == null) player = other.GetComponent<CelestePlayerController>();
            if (player != null) player.SetInputActive(false);

            // Start dialogue
            if (dialogueSystem != null) dialogueSystem.StartDialogueSequence();
        }
    }
}
