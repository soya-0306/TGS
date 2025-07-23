using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionalCardSelector : MonoBehaviour
{
    public int playerId; // 1P or 2P

    public DirectionalCardPair up, down, left, right;
    public SelectedOrderUI selectedOrderUI;
    public SelectionCounterUI selectionCounterUI;

    private List<Card> selectedCards = new List<Card>();
    private Deck playerDeck;
    public System.Action<int, List<Card>> OnSelectionComplete;

    [System.Serializable]
    public class DirectionalCardPair
    {
        public Button button;
        public Card card;
        public CardUI cardUI;
    }

    private DeckDefinition deck;

    private DeckInstance deckInstance;

    public DeckDefinition playerDeckDefinition;

    void Start()
    {

        // ここで DeckSelector から選ばれたデッキを渡す！
        if (playerId == 1)
        {
            deck = DeckSelector.Instance.player1Deck;
        }
        else if (playerId == 2)
        {
            deck = DeckSelector.Instance.player2Deck;
        }
        SetDeckDefinition(deck);

        //playerDeck = new Deck();

        playerDeck = CreateDeckFromDefinition(deck);  // DeckInstanceから実際のDeckを作る仮想的な処理例

        List<Card> drawnCards = playerDeck.DrawCards(4);

        if (drawnCards.Count >= 4)
        {
            up.card = drawnCards[0];
            down.card = drawnCards[1];
            left.card = drawnCards[2];
            right.card = drawnCards[3];
        }

        up.cardUI?.SetCard(up.card);
        down.cardUI?.SetCard(down.card);
        left.cardUI?.SetCard(left.card);
        right.cardUI?.SetCard(right.card);

        RefreshHand();
    }

    public Deck CreateDeckFromDefinition(DeckDefinition def)
    {
        return new Deck(def);
    }

    void Update()
    {
        // プレイヤーごとの操作定義（Input Manager で設定する必要あり）
        if (playerId == 1)
        {
            if (Input.GetKeyDown(KeyCode.W)) OnUpButtonPressed();
            if (Input.GetKeyDown(KeyCode.S)) OnDownButtonPressed();
            if (Input.GetKeyDown(KeyCode.A)) OnLeftButtonPressed();
            if (Input.GetKeyDown(KeyCode.D)) OnRightButtonPressed();
            
            if (Input.GetAxis("P1_Y") > 0)
            {
                //Debug.Log("OK");
                OnUpButtonPressed();
            }

            if (Input.GetAxis("P1_Y") < 0)
            {
                //Debug.Log("OK");
                OnDownButtonPressed();
            }

            if (Input.GetAxis("P1_X") > 0)
            {
                //Debug.Log("OK");
                OnRightButtonPressed();
            }

            if (Input.GetAxis("P1_X") < 0)
            {
                //Debug.Log("OK");
                OnLeftButtonPressed();
            }
        }

        else if (playerId == 2)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) OnUpButtonPressed();
            if (Input.GetKeyDown(KeyCode.DownArrow)) OnDownButtonPressed();
            if (Input.GetKeyDown(KeyCode.LeftArrow)) OnLeftButtonPressed();
            if (Input.GetKeyDown(KeyCode.RightArrow)) OnRightButtonPressed();

            if (Input.GetAxis("P2_Y") > 0)
            {
                //Debug.Log("OK");
                OnUpButtonPressed();
            }

            if (Input.GetAxis("P2_Y") < 0)
            {
                //Debug.Log("OK");
                OnDownButtonPressed();
            }

            if (Input.GetAxis("P2_X") > 0)
            {
                //Debug.Log("OK");
                OnRightButtonPressed();
            }

            if (Input.GetAxis("P2_X") < 0)
            {
                //Debug.Log("OK");
                OnLeftButtonPressed();
            }
        }
    }

    public void OnUpButtonPressed() => HandleInput(up.card, up.button);
    public void OnDownButtonPressed() => HandleInput(down.card, down.button);
    public void OnLeftButtonPressed() => HandleInput(left.card, left.button);
    public void OnRightButtonPressed() => HandleInput(right.card, right.button);

    private void HandleInput(Card card, Button button)
    {
        if (selectedCards.Contains(card) || selectedCards.Count >= 4)
            return;

        selectedCards.Add(card);
        button.interactable = false;

        //selectedOrderUI?.AddCardToSlot(selectedCards.Count - 1, card);
        selectionCounterUI?.UpdateSelectionCount(selectedCards.Count);

        Debug.Log($"Player {playerId} selected card: {card.Name}");

        if (selectedCards.Count == 4)
        {
            Debug.Log($"Player {playerId} selection complete.");
            OnSelectionComplete?.Invoke(playerId, selectedCards);
        }
    }

    public void RefreshHand()
    {

        Debug.Log($"playerDeckDefinition is null? {playerDeckDefinition == null}");
        if (playerDeckDefinition != null)
        {
            Debug.Log($"playerDeckDefinition.cards is null? {playerDeckDefinition.cards == null}");
            Debug.Log($"playerDeckDefinition.cards count: {playerDeckDefinition.cards.Count}");
        }

        playerDeck = new Deck(playerDeckDefinition);  // ← ここを修正

        List<Card> drawnCards = playerDeck.DrawCards(4);

        if (drawnCards.Count >= 4)
        {
            up.card = drawnCards[0];
            down.card = drawnCards[1];
            left.card = drawnCards[2];
            right.card = drawnCards[3];
        }

        up.cardUI?.SetCard(up.card);
        down.cardUI?.SetCard(down.card);
        left.cardUI?.SetCard(left.card);
        right.cardUI?.SetCard(right.card);

        ResetSelector();
    }


    public void ResetSelector()
    {
        selectedCards.Clear();

        up.button.interactable = true;
        down.button.interactable = true;
        left.button.interactable = true;
        right.button.interactable = true;

        //selectedOrderUI?.ResetSlots();
        selectionCounterUI?.ResetIndicators();
    }

    public void SetDeckDefinition(DeckDefinition def)
    {
        deckInstance = new DeckInstance(def);
        playerDeck = CreateDeckFromDefinition(deck);
    }
}
