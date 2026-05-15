using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("── Hedef ──")]
    [SerializeField] Transform target;

    [Header("── Takip ──")]
    [SerializeField] float smoothSpeed = 5f;
    [SerializeField] Vector2 offset    = new Vector2(0f, 2f);

    [Header("── Sınırlar ──")]
    [SerializeField] bool  useBounds = false;
    [SerializeField] float minX      = -100f;
    [SerializeField] float maxX      =  100f;
    [SerializeField] float minY      = -100f;
    [SerializeField] float maxY      =  100f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z);

        if (useBounds)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            desired.x = Mathf.Clamp(desired.x, minX + halfW, maxX - halfW);
            desired.y = Mathf.Clamp(desired.y, minY + halfH, maxY - halfH);
        }

        transform.position = Vector3.Lerp(
            transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}
