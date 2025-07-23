using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 出す順番のカード1つを表示するUIスロット
/// </summary>
public class CardSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;

    /// <summary>
    /// スロットにカードを設定
    /// </summary>
    public void SetCard(Card card)
    {
        if (card == null)
        {
            Clear(); // ← null のときはスロットを空にする
            return;
        }

        iconImage.sprite = card.Icon;
        nameText.text = card.Name;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// スロットを空にする
    /// </summary>
    public void Clear()
    {
        iconImage.sprite = null;
        nameText.text = "";
        gameObject.SetActive(false);
    }
}