using System.Collections;
using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    [Header("── Item ──")]
    [SerializeField] string     abilityName  = "Blink Dash";
    [SerializeField] string     keyHint      = "SHIFT";
    [SerializeField] Sprite     abilityIcon;           // Inspector'dan ata (opsiyonel)
    [SerializeField] float      bobSpeed     = 2f;    // yukarı aşağı sallanma hızı
    [SerializeField] float      bobHeight    = 0.15f; // sallanma yüksekliği
    [SerializeField] float      rotateSpeed  = 90f;   // dönme hızı

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Yukarı aşağı sal
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Döndür
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Blink Dash'i aç
        var blinkDash = other.GetComponent<BlinkDash>();
        if (blinkDash != null)
        {
            blinkDash.isUnlocked = true;
            AbilityHUD.Instance?.ShowAbility(abilityName, keyHint, abilityIcon);
        }

        // İleride başka yetenekler eklenebilir
        // var otherAbility = other.GetComponent<OtherAbility>();

        Destroy(gameObject);
    }
}
