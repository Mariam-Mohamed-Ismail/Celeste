using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Cinemachine;

public class CelesteDialogueSystem : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    
    [Header("Portraits")]
    public Sprite madelineSprite;
    public Sprite grannySprite;

    [Header("Audio")]
    public AudioSource audioSource;
    public float basePitch = 1.0f;

    [Header("Cameras & Interaction")]
    public CinemachineCamera gameplayCam;
    public CinemachineCamera cutsceneCam;
    public CelestePlayerController player;

    [System.Serializable]
    public class DialogueLine
    {
        public string speaker; // "Madeline" or "OldLady"
        [TextArea]
        public string text;
        public float preDelay = 0.5f;
        public float typeSpeed = 0.03f;
        public float postDelay = 1.0f;
    }

    public DialogueLine[] lines;
    private int index = 0;

    void Start()
    {
        if (panel != null) panel.SetActive(false);
        // Ensure we have an AudioSource and assign the procedural beep
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        if (audioSource.clip == null) audioSource.clip = CelesteVocalSynth.GenerateBeep();
    }

    public void StartDialogueSequence()
    {
        if (panel != null) panel.SetActive(true);
        index = 0;
        StartCoroutine(RunDialogueSequence());
    }

    IEnumerator RunDialogueSequence()
    {
        // Pause 1 second upon camera focus
        yield return new WaitForSeconds(1.0f);
        
        while (index < lines.Length)
        {
            var line = lines[index];
            
            // Switch Portrait
            UpdatePortrait(line.speaker);
            
            yield return new WaitForSeconds(line.preDelay);
            
            yield return StartCoroutine(TypeText(line.text, line.typeSpeed, line.speaker));
            
            yield return new WaitForSeconds(line.postDelay);
            index++;
        }
        
        // Phase 9: Exit
        EndDialogue();
    }

    void UpdatePortrait(string speaker)
    {
        if (portraitImage == null) return;
        
        if (string.Equals(speaker, "Madeline", System.StringComparison.OrdinalIgnoreCase))
        {
            portraitImage.sprite = madelineSprite;
            portraitImage.enabled = true;
        }
        else if (string.Equals(speaker, "OldLady", System.StringComparison.OrdinalIgnoreCase))
        {
            portraitImage.sprite = grannySprite;
            portraitImage.enabled = true;
        }
        else
        {
            portraitImage.enabled = false;
        }
    }

    IEnumerator TypeText(string textToType, float speed, string speaker)
    {
        if (dialogueText != null) {
            dialogueText.text = "";
            
            // Set dynamic pitch based on speaker
            float targetPitch = (string.Equals(speaker, "OldLady", System.StringComparison.OrdinalIgnoreCase)) ? 0.7f : 1.1f;
            audioSource.pitch = targetPitch;

            foreach (char c in textToType)
            {
                dialogueText.text += c;
                
                // Play vocal sound (randomize pitch slightly for extra juice)
                if (c != ' ') {
                    audioSource.pitch = targetPitch + Random.Range(-0.05f, 0.05f);
                    audioSource.PlayOneShot(audioSource.clip);
                }

                yield return new WaitForSeconds(speed);
            }
        }
    }

    void EndDialogue()
    {
        if (panel != null) panel.SetActive(false);
        if (gameplayCam != null) gameplayCam.Priority = 20;
        if (cutsceneCam != null) cutsceneCam.Priority = 10;
        if (player != null) player.SetInputActive(true);
    }
}
