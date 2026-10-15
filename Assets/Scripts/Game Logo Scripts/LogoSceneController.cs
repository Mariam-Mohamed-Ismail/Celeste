using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LogoSceneController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 2f;

    private bool isTransitioning = false;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && !isTransitioning)
        {
            StartCoroutine(LoadMainMenu());
        }
    }

    IEnumerator FadeIn()
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    IEnumerator LoadMainMenu()
    {
        isTransitioning = true;

        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }
}