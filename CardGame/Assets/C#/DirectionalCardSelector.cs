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

    public List<Card> ForceSelectRemainingCards()
    {
        List<Card> remaining = new List<Card>();

        if (!selectedCards.Contains(up.card)) remaining.Add(up.card);
        if (!selectedCards.Contains(down.card)) remaining.Add(down.card);
        if (!selectedCards.Contains(left.card)) remaining.Add(left.card);
        if (!selectedCards.Contains(right.card)) remaining.Add(right.card);

        // 残ってる中からランダムに選ぶ
        while (selectedCards.Count < 4 && remaining.Count > 0)
        {
            int randIndex = Random.Range(0, remaining.Count);
            Card selected = remaining[randIndex];
            selectedCards.Add(selected);
            remaining.RemoveAt(randIndex);
        }

        // イベント発火（選択完了扱いにする）
        OnSelectionComplete?.Invoke(playerId, selectedCards);

        return selectedCards;
    }

    public void ForceCompleteSelectionIfNeeded()
    {
        if (selectedCards.Count >= 4) return;

        List<Card> candidates = new List<Card>();

        if (!selectedCards.Contains(up.card)) candidates.Add(up.card);
        if (!selectedCards.Contains(down.card)) candidates.Add(down.card);
        if (!selectedCards.Contains(left.card)) candidates.Add(left.card);
        if (!selectedCards.Contains(right.card)) candidates.Add(right.card);

        while (selectedCards.Count < 4 && candidates.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, candidates.Count);
            Card randomCard = candidates[index];
            selectedCards.Add(randomCard);
            candidates.RemoveAt(index);

            // UI反映
            selectionCounterUI?.UpdateSelectionCount(selectedCards.Count);
        }

        OnSelectionComplete?.Invoke(playerId, new List<Card>(selectedCards));
    }

}
