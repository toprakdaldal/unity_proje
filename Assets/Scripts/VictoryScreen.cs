using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryScreen : MonoBehaviour
{
    public static VictoryScreen Instance;

    [Header("── UI ──")]
    [SerializeField] GameObject  root;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Text    victoryText;
    [SerializeField] Image       backdrop;

    [Header("── Süreler ──")]
    [SerializeField] float slowMoDuration = 1.5f;
    [SerializeField] float slowMoScale    = 0.25f; // slow motion derecesi
    [SerializeField] float fadeInDuration = 1.5f;
    [SerializeField] float holdDuration   = 3f;
    [SerializeField] float textRiseAmount = 50f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (root != null) root.SetActive(false);
    }

    public void Show()
    {
        if (root == null) return;
        root.SetActive(true);
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        // Slow motion
        Time.timeScale = slowMoScale;
        CameraController.Instance?.Shake(0.5f, 0.4f);
        yield return new WaitForSecondsRealtime(slowMoDuration);
        Time.timeScale = 0f; // ardından tamamen durdur

        Vector3 startTextPos = victoryText != null ? victoryText.transform.localPosition : Vector3.zero;
        Vector3 startBelow   = startTextPos - new Vector3(0f, textRiseAmount, 0f);

        if (victoryText != null) victoryText.transform.localPosition = startBelow;

        // Fade in + metin yukarı yükselsin
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t  = elapsed / fadeInDuration;

            if (canvasGroup != null) canvasGroup.alpha = t;
            if (victoryText != null)
                victoryText.transform.localPosition = Vector3.Lerp(startBelow, startTextPos, t);

            yield return null;
        }

        if (canvasGroup != null) canvasGroup.alpha = 1f;
        if (victoryText != null) victoryText.transform.localPosition = startTextPos;

        yield return new WaitForSecondsRealtime(holdDuration);
    }
}
