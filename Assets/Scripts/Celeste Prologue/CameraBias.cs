using UnityEngine;
using Unity.Cinemachine;

public class CameraBias : MonoBehaviour
{
    public CelestePlayerController player;
    public CinemachineCamera cam;

    private CinemachineFollow transposer;
    private Vector3 baseOffset = new Vector3(0, 5f, -10f);
    private float currentDirection = 1f;

    void Start()
    {
        if (cam == null) cam = GetComponent<CinemachineCamera>();
        if (cam != null) transposer = cam.GetComponent<CinemachineFollow>();
        if (player == null) player = FindFirstObjectByType<CelestePlayerController>();
    }

    void LateUpdate()
    {
        if (transposer == null || player == null) return;

        // Check both X and Z for movement to set direction bias
        if (player.velocity.magnitude > 0.1f)
        {
            // Use the horizontal velocity to decide where the camera should "lean"
            if (Mathf.Abs(player.velocity.x) > 0.1f) currentDirection = Mathf.Sign(player.velocity.x);
        }
        if (transposer == null || player == null) return;

        // Track intended direction smoothly
        if (player.velocity.x > 0.1f) currentDirection = 1f;
        else if (player.velocity.x < -0.1f) currentDirection = -1f;

        // Cinematic Drift: Tiny vertical motion = alive camera
        float drift = Mathf.Sin(Time.time * 0.5f) * 0.3f;

        // Target offset with direction bias and drift
        // X bias = direction * 1.5, Y includes base 5 + drift + slight centering tweak (Frame off-center)
        Vector3 targetOffset = baseOffset + new Vector3(currentDirection * 1.2f, drift + 0.5f, 0);

        // Smoothly interpolate the Tracking offset
        transposer.FollowOffset = Vector3.Lerp(
            transposer.FollowOffset,
            targetOffset,
            Time.deltaTime * 3f
        );
    }
}
