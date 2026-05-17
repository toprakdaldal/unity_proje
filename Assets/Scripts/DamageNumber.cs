using System.Collections;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] float riseSpeed  = 1.8f;
    [SerializeField] float lifetime   = 0.7f;
    [SerializeField] float spreadX    = 0.4f;  // hafif rastgele yatay kayma

    TMP_Text label;

    public void Init(int amount, Color color)
    {
        label      = GetComponent<TMP_Text>();
        label.text = amount.ToString();
        label.color = color;

        // Hafif rastgele yatay kayma
        transform.position += new Vector3(
            Random.Range(-spreadX, spreadX), 0f, 0f);

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float elapsed = 0f;
        Color startColor = label.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t  = elapsed / lifetime;

            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            label.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
