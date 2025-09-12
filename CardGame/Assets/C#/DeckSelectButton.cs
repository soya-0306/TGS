using UnityEngine;
using UnityEngine.UI;

public class DeckSelectButton : MonoBehaviour
{
    public DeckDefinition deck;  // インスペクターでデッキを指定

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        //button.onClick.AddListener(OnClick);
    }

    public void OnClick(int playerIndex)
    {
        DeckSelector.Instance.SelectDeck(deck, playerIndex);
        Debug.Log($"Player{playerIndex} がデッキ「{deck.deckName}」を選択しました");
    }
}
