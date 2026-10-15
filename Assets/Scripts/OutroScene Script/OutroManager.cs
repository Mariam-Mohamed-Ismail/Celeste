using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class OutroManager : MonoBehaviour
{
    [Header("Video Setup")]
    public VideoPlayer video;          // Your VideoPlayer
    public RawImage videoImage;        // RawImage displaying the video

    [Header("UI Elements")]
    public CanvasGroup text;           // Text CanvasGroup
    public CanvasGroup fade;           // Black fade CanvasGroup

    [Header("Settings")]
    public string mainMenuScene = "MainMenu";
    public float textDelay = 0.3f;     // Delay before showing text
    public float fadeDuration = 1f;    // Fade to black duration
    public float displayTextTime = 2f; // Time text stays visible before fade

    void Start()
    {
        // Start hidden
        text.alpha = 0f;
        fade.alpha = 0f;

        // Make sure UI GameObjects are active
        text.gameObject.SetActive(true);
        fade.gameObject.SetActive(true);

        // Assign RenderTexture to RawImage
        if (video.targetTexture != null)
            videoImage.texture = video.targetTexture;

        StartCoroutine(PlayVideoAndShowText());
    }

    private IEnumerator PlayVideoAndShowText()
    {
        // Prepare video
        video.Prepare();
        while (!video.isPrepared)
            yield return null;

        // Play video
        video.Play();

        // Optional: wait a bit before showing text
        yield return new WaitForSeconds(textDelay);

        // Fade in text over video
        yield return StartCoroutine(FadeCanvasGroup(text, fadeDuration));

        // Wait until video finishes
        while (video.isPlaying)
            yield return null;

        // Keep text visible for a short while
        yield return new WaitForSeconds(displayTextTime);

        // Fade black
        yield return StartCoroutine(FadeCanvasGroup(fade, fadeDuration));

        // Load Main Menu
        SceneManager.LoadScene(mainMenuScene);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cg.alpha = Mathf.Clamp01(t);
            yield return null;
        }
        cg.alpha = 1f;
    }
}