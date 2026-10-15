using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CutsceneController : MonoBehaviour
{
    public AudioSource Music;
    public TextMeshProUGUI dialogueText;
    public CanvasGroup textGroup;
    public UnityEngine.UI.Image fadeImage;

    public float typingSpeed = 0.04f;
    public float lineDelay = 2.5f;
    public float fadeSpeed = 1.5f;

    private string[] lines = new string[]
    {
        "This is it, Madeline.",
        "Just breathe.",
        "You can do this.",
        "Climb the mountain."
    };

    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        // Fade from black
        yield return StartCoroutine(FadeFromBlack());

        for (int i = 0; i < lines.Length; i++)
        {
            yield return StartCoroutine(TypeLine(lines[i]));
            yield return new WaitForSeconds(lineDelay);
            yield return StartCoroutine(FadeTextOut());
        }

        // Fade to black
        yield return StartCoroutine(FadeToBlack());
        Music.Stop();
        SceneManager.LoadScene("Celeste-Prologue");
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        textGroup.alpha = 1;

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator FadeTextOut()
    {
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime;
            textGroup.alpha = t;
            yield return null;
        }
    }

    IEnumerator FadeFromBlack()
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    IEnumerator FadeToBlack()
    {
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}