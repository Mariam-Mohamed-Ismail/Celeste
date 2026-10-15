using UnityEngine;
using Unity.Cinemachine;

public class IntroCameraSwitch : MonoBehaviour
{
    public CinemachineCamera introCam;
    public CinemachineCamera playerCam;
    public float delay = 4f;

    void Start()
    {
        if (introCam != null) introCam.Priority = 20;
        if (playerCam != null) playerCam.Priority = 10;

        Invoke("SwitchCamera", delay);
    }

    void SwitchCamera()
    {
        if (introCam != null) introCam.Priority = 10;
        if (playerCam != null) playerCam.Priority = 20;
    }
}
