using UnityEngine;
using TMPro;

public class RingPickup : MonoBehaviour
{
    [Header("── Yüzük ──")]
    [SerializeField] Ring ring;


    [Header("── İpucu UI ──")]
    [SerializeField] GameObject hintObject;   // "E: Al" + isim + açıklama paneli
    [SerializeField] TMP_Text   hintNameText;
    [SerializeField] TMP_Text   hintDescText;

    [Header("── Animasyon ──")]
    [SerializeField] float bobSpeed    = 2f;
    [SerializeField] float bobHeight   = 0.12f;
    [SerializeField] float rotateSpeed = 60f;

    Vector3 startPos;
    bool    playerNearby = false;
    bool    collected    = false;

    void Start()
    {
        startPos = transform.position;
        if (hintObject != null) hintObject.SetActive(false);
    }

    void Update()
    {
        // Bobbing animasyon
        float newY         = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);

        // Toplama
        if (playerNearby && !collected && InputBindings.GetKeyDown(InputAction.Interact))
            Collect();
    }

    void Collect()
    {
        if (collected) return;
        if (RingController.Instance == null) return;

        if (RingController.Instance.TryEquip(ring))
        {
            collected = true;
            AbilityHUD.Instance?.ShowAbility(ring.ringName, "", ring.icon);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        if (hintObject != null)
        {
            hintObject.SetActive(true);
            if (hintNameText != null) hintNameText.text = ring.ringName;
            if (hintDescText != null) hintDescText.text = ring.description;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        if (hintObject != null) hintObject.SetActive(false);
    }
}
