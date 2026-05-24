using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("── Parallax Çarpanı ──")]
    [Tooltip("0 = oyun dünyasıyla beraber hareket eder (önplan). 1 = kameraya yapışır (sabit görünür).")]
    [Range(0f, 1f)]
    [SerializeField] float parallaxFactor = 0.5f;

    [Header("── Sonsuz Tekrar ──")]
    [SerializeField] bool  loopHorizontally = false;
    [SerializeField] float manualWidth      = 0f; // 0 = sprite genişliğinden alır

    Transform   cam;
    Vector3     startPos;
    Vector3     camStartPos;
    float       textureWidth;
    Transform   leftCopy;
    Transform   rightCopy;

    void Start()
    {
        cam = Camera.main != null ? Camera.main.transform : null;
        startPos    = transform.position;
        camStartPos = cam != null ? cam.position : Vector3.zero;

        if (loopHorizontally)
        {
            var sr = GetComponent<SpriteRenderer>();
            textureWidth = manualWidth > 0f
                ? manualWidth
                : (sr != null ? sr.bounds.size.x : 0f);

            // 2 kopya oluştur (sol + sağ)
            if (textureWidth > 0f && sr != null)
            {
                leftCopy  = CreateClone(sr, -textureWidth);
                rightCopy = CreateClone(sr,  textureWidth);
            }
        }
    }

    Transform CreateClone(SpriteRenderer original, float xOffset)
    {
        GameObject clone = new GameObject(gameObject.name + "_Clone");
        clone.transform.SetParent(transform.parent);
        clone.transform.position    = transform.position + new Vector3(xOffset, 0f, 0f);
        clone.transform.localScale  = transform.localScale;

        var sr = clone.AddComponent<SpriteRenderer>();
        sr.sprite         = original.sprite;
        sr.sortingLayerID = original.sortingLayerID;
        sr.sortingOrder   = original.sortingOrder;
        sr.color          = original.color;
        sr.flipX          = original.flipX;
        sr.flipY          = original.flipY;

        return clone.transform;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        Vector3 camDelta = cam.position - camStartPos;
        float targetX = startPos.x + camDelta.x * parallaxFactor;

        // Loop için: hedef X'i kameraya yakın olan tile'a kaydır
        if (loopHorizontally && textureWidth > 0f)
        {
            float diff = targetX - cam.position.x;
            targetX -= Mathf.Floor(diff / textureWidth + 0.5f) * textureWidth;
        }

        transform.position = new Vector3(targetX, startPos.y, startPos.z);

        // Kopyaları yan yana hizala
        if (loopHorizontally && leftCopy != null && rightCopy != null)
        {
            leftCopy.position  = transform.position + new Vector3(-textureWidth, 0f, 0f);
            rightCopy.position = transform.position + new Vector3( textureWidth, 0f, 0f);
        }
    }
}
