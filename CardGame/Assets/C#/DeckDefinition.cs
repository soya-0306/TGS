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
    public List<CardType> sequence;  // �R���{�����ɕK�v�ȃJ�[�h�̕���
    public float damageMultiplier = 1.5f; // �{��
    public string comboName;
}

[CreateAssetMenu(fileName = "NewDeck", menuName = "CardGame/Deck")]
public class DeckDefinition : ScriptableObject
{
    public string deckName;
    public Sprite deckIcon;
    public List<CardEntry> cards;  // ������ List<CardEntry> �ɂ���

    public List<Combo> combos = new List<Combo>();  // �f�b�L�ŗL�̃R���{�������ɐݒ�

    [Header("�ėp�R���{")]
    public List<Combo> genericCombos = new List<Combo>();  // �ėp�R���{�͂����ɂ܂Ƃ߂�

    [Header("�f�b�L�L�����N�^�[")]
    public GameObject deckCharacterPrefab; // �L�����N�^�[��Prefab�������ɃZ�b�g�ł���悤��
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

        // 1. �������2�A���ŏo�����i�U���n�̂݁j
        if (lastCard.Type == currentCard.Type &&
            currentCard.IsAttackCard() && lastCard.IsAttackCard())
        {
            return 2.0f;
        }

        // 2. �V�[���h�̌�̍U��
        if (lastCard.IsShield() && currentCard.IsAttackCard())
        {
            return 2.0f;
        }

        return 1.0f;
    }
}
