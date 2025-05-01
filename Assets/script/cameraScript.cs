using UnityEngine;

public class cameraScript : MonoBehaviour
{
    public GameObject player;
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (player == null) return;
        Vector3 desiredPosition = player.transform.position + new Vector3(0, 0, -10);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
