using UnityEngine;

public class CutsceneCameraDrift : MonoBehaviour
{
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float drift = Mathf.Sin(Time.time * 0.5f) * 0.2f;
        transform.position = initialPosition + new Vector3(0, drift, 0);
    }
}
