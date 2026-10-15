using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOutroScene : MonoBehaviour
{
    public AudioSource sceneMusic;
    public string outroSceneName = "OutroScene";
    public CanvasGroup fadeCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeAndLoad());
        }
    }

    System.Collections.IEnumerator FadeAndLoad()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = t;
            yield return null;
        }
        sceneMusic.Stop();
        SceneManager.LoadScene(outroSceneName);
    }
}