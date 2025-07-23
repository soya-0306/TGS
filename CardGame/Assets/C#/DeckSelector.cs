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
            DontDestroyOnLoad(gameObject);  // ÉVÅ[Éìà⁄ìÆÇ≈îjä¸Ç≥ÇÍÇ»Ç¢ÇÊÇ§Ç…
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectDeck(DeckDefinition deck)
    {
        if (currentSelecting == PlayerSelecting.Player1)
        {
            player1Deck = deck;
            Debug.Log($"Player1 selected: {deck.deckName}");
        }
        else
        {
            player2Deck = deck;
            Debug.Log($"Player2 selected: {deck.deckName}");
        }
    }

    public void TogglePlayer()
    {
        currentSelecting = (currentSelecting == PlayerSelecting.Player1)
            ? PlayerSelecting.Player2
            : PlayerSelecting.Player1;
    }
}
