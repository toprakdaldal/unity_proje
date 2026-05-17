using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("── Hedef ──")]
    [SerializeField] Transform target;

    [Header("── Takip ──")]
    [SerializeField] float   smoothSpeed = 5f;
    [SerializeField] Vector2 offset      = new Vector2(0f, 2f);

    [Header("── Sınırlar ──")]
    [SerializeField] bool  useBounds = false;
    [SerializeField] float minX      = -100f;
    [SerializeField] float maxX      =  100f;
    [SerializeField] float minY      = -100f;
    [SerializeField] float maxY      =  100f;

    Camera  cam;
    Vector3 shakeOffset;

    void Awake()
    {
        Instance = this;
        cam      = GetComponent<Camera>();
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
            transform.position, desired, smoothSpeed * Time.deltaTime) + shakeOffset;
    }

    // ── Ekran Sarsıntısı ─────────────────────────────────────
    public void Shake(float duration = 0.15f, float magnitude = 0.12f)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed     += Time.unscaledDeltaTime;
            float t      = 1f - (elapsed / duration); // zamanla azalır
            shakeOffset  = new Vector3(
                Random.Range(-1f, 1f) * magnitude * t,
                Random.Range(-1f, 1f) * magnitude * t,
                0f);
            yield return null;
        }
        shakeOffset = Vector3.zero;
    }
}
