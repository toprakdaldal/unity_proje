using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardChoiceUI : MonoBehaviour
{
    public static CardChoiceUI Instance;

    [Header("── Panel ──")]
    [SerializeField] GameObject panel;

    [Header("── 3 Kart Slotu ──")]
    [SerializeField] Image[]    cardBackgrounds; // 3 image (kart kutuları)
    [SerializeField] TMP_Text[] cardNameTexts;   // 3 isim
    [SerializeField] TMP_Text[] cardDescTexts;   // 3 açıklama
    [SerializeField] Button[]   cardButtons;     // 3 buton

    [Header("── Şu An Aktif Kart Etiketi (opsiyonel) ──")]
    [SerializeField] TMP_Text activeCardLabel;

    List<Card> currentChoices = new List<Card>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (panel != null) panel.SetActive(false);

        // Buton bağla
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i; // closure
            if (cardButtons[i] != null)
                cardButtons[i].onClick.AddListener(() => OnCardClicked(idx));
        }
    }

    public void Show()
    {
        if (panel == null || CardSystem.Instance == null) return;
        currentChoices = CardSystem.Instance.DrawRandom(3);

        for (int i = 0; i < 3; i++)
        {
            if (i >= currentChoices.Count)
            {
                if (cardButtons[i] != null) cardButtons[i].gameObject.SetActive(false);
                continue;
            }

            Card c = currentChoices[i];
            if (cardButtons[i] != null) cardButtons[i].gameObject.SetActive(true);
            if (cardBackgrounds[i] != null) cardBackgrounds[i].color = c.cardColor;
            if (cardNameTexts[i]   != null) cardNameTexts[i].text    = c.cardName;
            if (cardDescTexts[i]   != null) cardDescTexts[i].text    = c.description;
        }

        Time.timeScale = 0f;
        panel.SetActive(true);
    }

    void OnCardClicked(int index)
    {
        if (index < 0 || index >= currentChoices.Count) return;
        CardSystem.Instance?.Select(currentChoices[index]);

        if (activeCardLabel != null && CardSystem.Instance?.activeCard != null)
            activeCardLabel.text = $"Aktif: {CardSystem.Instance.activeCard.cardName}";

        Close();
    }

    public void Close()
    {
        Time.timeScale = 1f;
        if (panel != null) panel.SetActive(false);
    }
}
