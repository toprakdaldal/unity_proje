using UnityEngine;

// Düşmanlardan düşen hayalet taşı — toplayınca 1 charge ekler
public class GhostStepPickup : MonoBehaviour
{
    [SerializeField] float bobSpeed    = 2.5f;
    [SerializeField] float bobHeight   = 0.12f;
    [SerializeField] float rotateSpeed = 80f;
    [SerializeField] float lifetime    = 8f;  // toplanmazsa yok olur

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        float newY         = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var ghostStep = other.GetComponent<GhostStep>();
        if (ghostStep != null)
            ghostStep.AddCharge(1);

        Destroy(gameObject);
    }
}
