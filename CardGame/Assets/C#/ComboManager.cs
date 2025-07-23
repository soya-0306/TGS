using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Combo
{
    public List<CardType> sequence;  // ���肵�����J�[�h�^�C�v�̘A����
    public float damageMultiplier;   // �R���{�K�p���̔{��

    public int effectID; // �������ID (0 = ����)

    public bool isOrderless; // ���s���R���{���ǂ����̃t���O

    public Combo(List<CardType> seq, float multiplier)
    {
        sequence = seq;
        damageMultiplier = multiplier;
        isOrderless = false;
    }
}

public class ComboManager
{
    private List<Combo> combos;            // �ŗL�R���{
    private List<Combo> genericCombos;     // �ėp�R���{

    public ComboManager(List<Combo> combosFromDeck, List<Combo> genericCombosFromDeck)
    {
        combos = combosFromDeck;
        genericCombos = genericCombosFromDeck;
    }

    /// <summary>
    /// �O�̃J�[�h�ƍ��̃J�[�h�����ăR���{���������Ă�����{����Ԃ�
    /// �ŗL�R���{���ėp�R���{�̏��Ŕ���
    /// </summary>
    public float GetDamageMultiplier(List<Card> prevCards, Card currentCard, PlayerHP currentPlayer)
    {
        if (currentCard == null)
            return 1.0f;

        // �ŗL�R���{����
        foreach (var combo in combos)
        {
            int comboLength = combo.sequence.Count;
            if (comboLength < 2 || comboLength > 4)
                continue;

            if (prevCards.Count < comboLength - 1)
                continue;

            List<CardType> recentTypes = new List<CardType>();
            for (int i = prevCards.Count - (comboLength - 1); i < prevCards.Count; i++)
            {
                recentTypes.Add(prevCards[i].Type);
            }
            recentTypes.Add(currentCard.Type);

            if (combo.isOrderless)
            {
                List<CardType> comboSorted = new List<CardType>(combo.sequence);
                List<CardType> recentSorted = new List<CardType>(recentTypes);
                comboSorted.Sort();
                recentSorted.Sort();

                bool matchOrderless = true;
                for (int i = 0; i < comboLength; i++)
                {
                    if (comboSorted[i] != recentSorted[i])
                    {
                        matchOrderless = false;
                        break;
                    }
                }

                if (matchOrderless)
                {
                    ComboEffectManager.ApplyEffect(combo.effectID, currentPlayer);
                    return combo.damageMultiplier;
                }
            }
            else
            {
                bool match = true;
                for (int i = 0; i < comboLength - 1; i++)
                {
                    if (prevCards[prevCards.Count - (comboLength - 1) + i].Type != combo.sequence[i])
                    {
                        match = false;
                        break;
                    }
                }

                if (!match)
                    continue;

                if (currentCard.Type == combo.sequence[comboLength - 1])
                {
                    ComboEffectManager.ApplyEffect(combo.effectID, currentPlayer);
                    return combo.damageMultiplier;
                }
            }
        }

        // �ėp�R���{����
        float genericMultiplier = CheckGenericCombos(prevCards, currentCard);
        if (genericMultiplier > 1.0f)
        {
            // �ėp�R���{�ɂ͓�����ʂȂ��Ŕ{�������Ԃ��ꍇ�͂��̂܂ܕԂ�
            return genericMultiplier;
        }

        return 1.0f;
    }

    /// <summary>
    /// �ėp�R���{����i������A���A�V�[���h���U���Ȃǁj
    /// </summary>
    private float CheckGenericCombos(List<Card> prevCards, Card currentCard)
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

    /// <summary>
    /// �R���{���̃��X�g��Ԃ��iUI�\���p�Ȃǁj
    /// �ŗL�R���{�̂ݔ���
    /// </summary>
    public List<Combo> GetMatchingCombos(List<Card> currentCards)
    {
        List<Combo> matched = new List<Combo>();

        foreach (var combo in combos)
        {
            if (combo.sequence.Count > currentCards.Count)
                continue;

            List<CardType> cardTypes = new List<CardType>();
            for (int i = currentCards.Count - combo.sequence.Count; i < currentCards.Count; i++)
            {
                cardTypes.Add(currentCards[i].Type);
            }

            if (combo.isOrderless)
            {
                List<CardType> comboSorted = new List<CardType>(combo.sequence);
                List<CardType> cardsSorted = new List<CardType>(cardTypes);
                comboSorted.Sort();
                cardsSorted.Sort();

                bool match = true;
                for (int i = 0; i < combo.sequence.Count; i++)
                {
                    if (comboSorted[i] != cardsSorted[i])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    matched.Add(combo);
                }
            }
            else
            {
                bool match = true;
                for (int i = 0; i < combo.sequence.Count; i++)
                {
                    if (combo.sequence[i] != cardTypes[i])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    matched.Add(combo);
                }
            }
        }
        return matched;
    }
}
