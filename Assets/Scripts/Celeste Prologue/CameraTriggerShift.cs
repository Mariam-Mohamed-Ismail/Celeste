using UnityEngine;
using Unity.Cinemachine;

public class CameraTriggerShift : MonoBehaviour
{
    public CinemachineCamera cam;
    public Vector3 newOffset;

    private CinemachinePositionComposer transposer;

    void Start()
    {
        if (cam != null)
            transposer = cam.GetComponent<CinemachinePositionComposer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && transposer != null)
        {
            transposer.TargetOffset = newOffset;
        }
    }
}
