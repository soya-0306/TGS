using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �R�D�i�f�b�L�j�𐶐��E�Ǘ�����N���X
/// </summary>
public class Deck
{
    private List<Card> cards;

    public Card currentCard;

    // ��R���X�g���N�^
    public Deck()
    {
        cards = new List<Card>();
    }

    // DeckDefinition ������R���X�g���N�^�i�܂��̓t�@�N�g�����\�b�h�j
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
    /// �J�[�h�̎�ނɉ����ĉ摜��ǂݍ���
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
    /// �^�C�v���Ƃ̕\���p���O��Ԃ�
    /// </summary>
    private string GetCardName(CardType type)
    {
        switch (type)
        {
            case CardType.Rock: return "rock";
            case CardType.Scissors: return "scissors";
            case CardType.Paper: return "paper";
            case CardType.Shield: return "shield";
            default: return "�H�H�H";
        }
    }

    /// <summary>
    /// �R�D�������_���ɕ��ёւ���
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
    /// �R�D����w�薇���̃J�[�h���h���[
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
    /// �c�薇�����擾
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
