using UnityEngine;
using TMPro;

public class TimeFreezeTutorial : MonoBehaviour
{
    public GameObject tutorialUI;
    private bool isFrozen = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFrozen)
        {
            Freeze();
        }
    }

    void Freeze()
    {
        isFrozen = true;
        Time.timeScale = 0f;
        if (tutorialUI != null) tutorialUI.SetActive(true);
    }

    void Update()
    {
        if (isFrozen)
        {
            // Unfreeze when the user tries to Dash (per UI hint)
            bool dash = Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetMouseButtonDown(1);
            bool jump = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space);

            if (dash || jump)
            {
                Unfreeze();
            }
        }
    }

    void Unfreeze()
    {
        isFrozen = false;
        Time.timeScale = 1f;
        if (tutorialUI != null) tutorialUI.SetActive(false);
    }
}
