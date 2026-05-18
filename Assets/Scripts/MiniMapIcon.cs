using UnityEngine;

// Mini haritada görünmesi gereken sembolleri (oyuncu, checkpoint vb.) belirler
[RequireComponent(typeof(SpriteRenderer))]
public class MiniMapIcon : MonoBehaviour
{
    [Header("── Mini Harita Görseli ──")]
    [SerializeField] Color iconColor = Color.white;
    [SerializeField] float iconScale = 1.5f;

    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.color = iconColor;
        transform.localScale = Vector3.one * iconScale;
    }
}
