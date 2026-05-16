using System.Collections;
using UnityEngine;

public class BlinkDash : MonoBehaviour
{
    [Header("── Blink Dash ──")]
    [SerializeField] float ghostDuration = 0.3f;
    [SerializeField] float ghostInterval = 0.08f;
    [SerializeField] float decoyRadius   = 6f;
    [SerializeField] Color ghostColor    = new Color(0.4f, 0.8f, 1f, 0.6f);

    [HideInInspector] public bool isUnlocked = false;

    SpriteRenderer   sr;
    PlayerController playerController;
    float            ghostTimer;
    bool             wasDAashing;

    void Awake()
    {
        sr               = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!isUnlocked) return;
        if (!playerController.IsDashing) { wasDAashing = false; return; }

        ghostTimer -= Time.deltaTime;
        if (ghostTimer <= 0f)
        {
            ghostTimer = ghostInterval;
            SpawnGhost();
        }
        wasDAashing = true;
    }

    void SpawnGhost()
    {
        if (sr == null || sr.sprite == null) return;

        GameObject ghost = new GameObject("Ghost");
        ghost.transform.position   = transform.position;
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer ghostSR   = ghost.AddComponent<SpriteRenderer>();
        ghostSR.sprite           = sr.sprite;
        ghostSR.color            = ghostColor;
        ghostSR.sortingOrder     = sr.sortingOrder - 1;

        var decoy = ghost.AddComponent<GhostDecoy>();
        decoy.decoyDuration = ghostDuration;
        decoy.decoyRadius   = decoyRadius;
        StartCoroutine(FadeGhost(ghostSR, ghostDuration));
    }

    IEnumerator FadeGhost(SpriteRenderer ghostSR, float duration)
    {
        float elapsed = 0f;
        Color start   = ghostSR.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (ghostSR == null) yield break;
            float alpha  = Mathf.Lerp(start.a, 0f, elapsed / duration);
            ghostSR.color = new Color(start.r, start.g, start.b, alpha);
            yield return null;
        }

        if (ghostSR != null) Destroy(ghostSR.gameObject);
    }
}
