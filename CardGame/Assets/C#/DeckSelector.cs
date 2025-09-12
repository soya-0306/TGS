using UnityEngine;

public enum PlayerSelecting
{
    Player1,
    Player2
}

public class DeckSelector : MonoBehaviour
{
    public static DeckSelector Instance;

    public DeckDefinition player1Deck;
    public DeckDefinition player2Deck;

    public PlayerSelecting currentSelecting = PlayerSelecting.Player1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // シーン移動で破棄されないように
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ✅ プレイヤー番号を明示して保存する
    public void SelectDeck(DeckDefinition deck, int playerIndex)
    {
        if (playerIndex == 1)
        {
            player1Deck = deck;
            Debug.Log($"Player1 selected: {deck.deckName}");
        }
        else if (playerIndex == 2)
        {
            player2Deck = deck;
            Debug.Log($"Player2 selected: {deck.deckName}");
        }
    }
}
