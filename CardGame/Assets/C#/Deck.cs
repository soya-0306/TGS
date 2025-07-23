using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 山札（デッキ）を生成・管理するクラス
/// </summary>
public class Deck
{
    private List<Card> cards;

    public Card currentCard;

    // 空コンストラクタ
    public Deck()
    {
        cards = new List<Card>();
    }

    // DeckDefinition から作るコンストラクタ（またはファクトリメソッド）
    public Deck(DeckDefinition deckDefinition)
    {
        cards = new List<Card>();

        foreach (var entry in deckDefinition.cards)
        {
            for (int i = 0; i < entry.count; i++)
            {
                string displayName = GetCardName(entry.cardType);
                Sprite icon = LoadIconForType(entry.cardType);
                cards.Add(new Card(entry.cardType, displayName, icon));
            }
        }

        Shuffle();
    }

    /// <summary>
    /// カードの種類に応じて画像を読み込む
    /// </summary>
    private Sprite LoadIconForType(CardType type)
    {
        switch (type)
        {
            case CardType.Rock:
                return Resources.Load<Sprite>("Sprites/rock");
            case CardType.Scissors:
                return Resources.Load<Sprite>("Sprites/scissors");
            case CardType.Paper:
                return Resources.Load<Sprite>("Sprites/paper");
            case CardType.Shield:
                return Resources.Load<Sprite>("Sprites/shield");
            default:
                return null;
        }
    }


    /// <summary>
    /// タイプごとの表示用名前を返す
    /// </summary>
    private string GetCardName(CardType type)
    {
        switch (type)
        {
            case CardType.Rock: return "rock";
            case CardType.Scissors: return "scissors";
            case CardType.Paper: return "paper";
            case CardType.Shield: return "shield";
            default: return "？？？";
        }
    }

    /// <summary>
    /// 山札をランダムに並び替える
    /// </summary>
    public void Shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cards.Count);
            Card temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 山札から指定枚数のカードをドロー
    /// </summary>
    public List<Card> DrawCards(int count)
    {
        List<Card> drawn = new List<Card>();
        for (int i = 0; i < count && cards.Count > 0; i++)
        {
            drawn.Add(cards[0]);
            cards.RemoveAt(0);
        }
        return drawn;
    }

    /// <summary>
    /// 残り枚数を取得
    /// </summary>
    public int RemainingCount()
    {
        return cards.Count;
    }

    public Card DrawRandomCard()
    {
        if (cards.Count == 0) return null;

        int randomIndex = Random.Range(0, cards.Count);
        Card drawnCard = cards[randomIndex];
        cards.RemoveAt(randomIndex);
        return drawnCard;
    }

}
