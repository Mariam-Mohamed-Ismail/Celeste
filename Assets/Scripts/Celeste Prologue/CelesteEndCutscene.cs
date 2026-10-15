using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CelesteEndCutscene : MonoBehaviour
{
    [Header("References")]
    public PlayableDirector timeline;
    public GameObject endTrigger;

    private bool cutsceneStarted = false;

    void OnEnable()
    {
        if (timeline == null) timeline = GetComponent<PlayableDirector>();
        if (timeline != null) timeline.stopped += OnTimelineStopped;
    }

    void OnDisable()
    {
        if (timeline != null) timeline.stopped -= OnTimelineStopped;
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        LoadMainMenu();
    }

    void OnTriggerEnter(Collider other)
    {
        if (cutsceneStarted) return;

        if (other.CompareTag("Player"))
        {
            StartCutscene(other.gameObject);
        }
    }

    public void StartCutscene(GameObject player)
    {
        cutsceneStarted = true;

        // 0. Disable gameplay UI
        var fade = GameObject.Find("FadeCanvas");
        if (fade != null) fade.SetActive(false);
        var dialogue = GameObject.Find("DialogueCanvas");
        if (dialogue != null) dialogue.SetActive(false);

        // 1. Lock Player Input & Play Idle Animation
        CelestePlayerController controller = player.GetComponent<CelestePlayerController>();
        if (controller != null)
        {
            controller.PlayBreathingAnimation();
        }

        // 2. Play Timeline
        if (timeline != null)
        {
            timeline.Play();
        }

        // 3. Disable the trigger visual/object if needed
        if (endTrigger != null)
        {
            endTrigger.SetActive(false);
        }

        Debug.Log("End Cutscene Started!");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
