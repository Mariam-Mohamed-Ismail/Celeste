using UnityEngine;

public class CameraMousePeek : MonoBehaviour
{
    [Header("Target Configuration")]
    public Transform playerTarget;
    
    [Header("Mouse Peek Settings")]
    public float maxPeekDistanceX = 6f;
    public float maxPeekDistanceY = 4f;
    public float smoothSpeed = 3f;

    private Vector3 currentOffset;

    void LateUpdate()
    {
        if (playerTarget == null) return;

        // Base position is the Player
        Vector3 targetPos = playerTarget.position;

        // Calculate Mouse Offset from Center of Screen (-1 to 1)
        Vector3 mousePos = Input.mousePosition;
        float screenOffsetX = (mousePos.x / Screen.width) * 2f - 1f;
        float screenOffsetY = (mousePos.y / Screen.height) * 2f - 1f;

        // Clamp offsets
        screenOffsetX = Mathf.Clamp(screenOffsetX, -1f, 1f);
        screenOffsetY = Mathf.Clamp(screenOffsetY, -1f, 1f);

        // Desired peek world offset Z is purely constrained to the distance formula but we only need X/Y
        Vector3 desiredOffset = new Vector3(-screenOffsetX * maxPeekDistanceX, screenOffsetY * maxPeekDistanceY, 0f);

        // Smoothly interpolate current offset towards desired offset
        currentOffset = Vector3.Lerp(currentOffset, desiredOffset, Time.deltaTime * smoothSpeed);

        // Apply final position to this target object which Cinemachine is following!
        transform.position = targetPos + currentOffset;
    }
}
