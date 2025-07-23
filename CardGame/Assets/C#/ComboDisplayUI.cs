using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class ComboDisplayUI : MonoBehaviour
{
    [SerializeField] private GameObject comboPanelPrefab;  // コンボ表示パネルのプレハブ
    [SerializeField] private Transform comboPanelParent;   // パネルの親Transform
    [SerializeField] private GameObject cardIconPrefab;    // カードアイコン（Image）プレハブ

    private List<GameObject> activeComboPanels = new List<GameObject>();

    /// <summary>
    /// コンボのリストを受け取ってUI表示を更新する
    /// </summary>
    public void ShowCombos(List<Combo> combosToShow, Dictionary<CardType, Sprite> cardIconTable)
    {
        ClearComboPanels();

        foreach (var combo in combosToShow)
        {
            GameObject panel = Instantiate(comboPanelPrefab, comboPanelParent);
            activeComboPanels.Add(panel);

            // パネル内にコンボ名テキストとカードアイコンをセットする想定
            Text comboNameText = panel.transform.Find("ComboNameText").GetComponent<Text>();
            comboNameText.text = combo.sequence.Count > 0 ? $"Combo: {combo.sequence.Count} cards" : "Combo";

            // カードアイコンを順に表示
            Transform cardsParent = panel.transform.Find("Cards");
            foreach (var cardType in combo.sequence)
            {
                GameObject iconGO = Instantiate(cardIconPrefab, cardsParent);
                Image iconImage = iconGO.GetComponent<Image>();

                if (cardIconTable.TryGetValue(cardType, out var iconSprite))
                {
                    iconImage.sprite = iconSprite;
                }
                else
                {
                    Debug.LogWarning($"CardType {cardType} のアイコンが見つかりません。");
                }
            }
        }
    }

    /// <summary>
    /// 既存のコンボパネルを全部消す
    /// </summary>
    public void ClearComboPanels()
    {
        foreach (var panel in activeComboPanels)
        {
            Destroy(panel);
        }
        activeComboPanels.Clear();
    }
}
