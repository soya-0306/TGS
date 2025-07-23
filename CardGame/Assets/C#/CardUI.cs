using UnityEngine;
using UnityEngine.UI;
using TMPro; // ← これを追加

/// <summary>
/// 1枚のカードUIを管理するクラス（表示とデータの紐付け）
/// </summary>
public class CardUI : MonoBehaviour
{
    [Header("UI参照")]
    public Image iconImage;     // カードのアイコン（Imageコンポーネント）
    public TMP_Text nameText;       // カードの名前（Textコンポーネント）

    private Card cardData;      // 表示対象のカードデータ

    /// <summary>
    /// 表示するカードのデータを設定
    /// </summary>
    public void SetCard(Card card)
    {
        cardData = card;

        // UIを更新
        if (iconImage != null)
        {
            iconImage.sprite = card.Icon;
        }

        if (nameText != null)
        {
            nameText.text = card.Name;
        }
    }

    /// <summary>
    /// このUIカードが持っているカードデータを返す
    /// </summary>
    public Card GetCard()
    {
        return cardData;
    }
}
