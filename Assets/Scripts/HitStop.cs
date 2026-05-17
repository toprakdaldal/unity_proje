using System.Collections;
using UnityEngine;

// Sahneye boş bir objeye ekle — singleton
public class HitStop : MonoBehaviour
{
    public static HitStop Instance;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>duration saniye boyunca oyunu dondurur (unscaled time kullanır)</summary>
    public void Stop(float duration = 0.06f)
    {
        StartCoroutine(StopRoutine(duration));
    }

    IEnumerator StopRoutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
