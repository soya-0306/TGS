using UnityEngine;
using UnityEngine.UI;

public class DeckSelectButton : MonoBehaviour
{
    public DeckDefinition deck;  // インスペクターでデッキを指定

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        DeckSelector.Instance.SelectDeck(deck);
        Debug.Log($"デッキ「{deck.deckName}」を選択しました");

        // プレイヤー切り替え（任意。1P→2Pへ）
        DeckSelector.Instance.TogglePlayer();
    }
}
