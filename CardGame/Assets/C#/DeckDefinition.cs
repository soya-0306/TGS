using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

[System.Serializable]
public class CardEntry
{
    public CardType cardType;
    public int count;
}

[System.Serializable]
public class ComboPattern
{
    public List<CardType> sequence;  // コンボ成立に必要なカードの並び
    public float damageMultiplier = 1.5f; // 倍率
    public string comboName;
}

[CreateAssetMenu(fileName = "NewDeck", menuName = "CardGame/Deck")]
public class DeckDefinition : ScriptableObject
{
    public string deckName;
    public Sprite deckIcon;
    public List<CardEntry> cards;  // ここを List<CardEntry> にする

    public List<Combo> combos = new List<Combo>();  // デッキ固有のコンボをここに設定

    [Header("汎用コンボ")]
    public List<Combo> genericCombos = new List<Combo>();  // 汎用コンボはここにまとめる

    [Header("デッキキャラクター")]
    public GameObject deckCharacterPrefab; // キャラクターのPrefabをここにセットできるように
}

public class GenericComboManager
{
    private List<Combo> genericCombos;

    public GenericComboManager(List<Combo> combosFromDeck)
    {
        genericCombos = combosFromDeck;
    }

    public float CheckGenericCombos(List<Card> prevCards, Card currentCard)
    {
        if (prevCards.Count == 0) return 1.0f;

        Card lastCard = prevCards[prevCards.Count - 1];

        // 1. 同じ手を2連続で出した（攻撃系のみ）
        if (lastCard.Type == currentCard.Type &&
            currentCard.IsAttackCard() && lastCard.IsAttackCard())
        {
            return 2.0f;
        }

        // 2. シールドの後の攻撃
        if (lastCard.IsShield() && currentCard.IsAttackCard())
        {
            return 2.0f;
        }

        return 1.0f;
    }
}
