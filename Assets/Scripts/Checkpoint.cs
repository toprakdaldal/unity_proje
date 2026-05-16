using UnityEngine;
using TMPro;

public class Checkpoint : MonoBehaviour
{
    [Header("── Görsel ──")]
    [SerializeField] SpriteRenderer  flameRenderer;
    [SerializeField] ParticleSystem  activateParticles;
    [SerializeField] Color           inactiveColor = new Color(0.35f, 0.35f, 0.45f);
    [SerializeField] Color           activeColor   = new Color(1f, 0.85f, 0.3f);

    [Header("── UI İpucu ──")]
    [SerializeField] GameObject      hintObject;  // "E: Dinlen" yazısı

    bool playerNearby = false;
    bool isActivated  = false;

    void Start()
    {
        if (flameRenderer != null) flameRenderer.color = inactiveColor;
        if (hintObject    != null) hintObject.SetActive(false);
    }

    void Update()
    {
        if (!playerNearby) return;
        if (Input.GetKeyDown(KeyCode.E))
            Activate();
    }

    void Activate()
    {
        isActivated = true;

        // Checkpoint kaydet
        CheckpointManager.Instance?.SetCheckpoint(transform.position);

        // Oyuncuyu iyileştir
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        playerObj?.GetComponent<HealthController>()?.Heal(9999);

        // Görsel
        if (flameRenderer      != null) flameRenderer.color = activeColor;
        if (activateParticles  != null) activateParticles.Play();
        if (hintObject         != null) hintObject.SetActive(false);

        // Diğer checkpoint'leri söndür
        foreach (var cp in FindObjectsByType<Checkpoint>(FindObjectsSortMode.None))
            if (cp != this) cp.Deactivate();
    }

    public void Deactivate()
    {
        isActivated = false;
        if (flameRenderer != null) flameRenderer.color = inactiveColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        if (hintObject != null) hintObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        if (hintObject != null) hintObject.SetActive(false);
    }
}
