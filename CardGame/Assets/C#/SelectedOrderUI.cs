using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedOrderUI : MonoBehaviour
{
    public Image[] cardImages;     // 表示スロット（最大4つ）
    public TextMeshProUGUI[] cardTexts; // 名前表示（任意）

    public void AddCardToSlot(int index, Card card)
    {
        if (index >= cardImages.Length) return;

        cardImages[index].sprite = card.Icon;
        cardImages[index].color = Color.white; // 非表示防止

        if (cardTexts != null && index < cardTexts.Length)
        {
            cardTexts[index].text = card.Name;
        }
    }

/// <summary>
/// UI初期化（リセット）
/// </summary>
    //public void ClearAll()
    //{
    //    foreach (var slot in cardImages)
    //    {
    //        slot.Clear();
    //    }
    //}
}
