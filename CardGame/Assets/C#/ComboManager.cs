using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Combo
{
    public List<CardType> sequence;  // 判定したいカードタイプの連続列
    public float damageMultiplier;   // コンボ適用時の倍率

    public int effectID; // 特殊効果ID (0 = 無し)

    public bool isOrderless; // 順不同コンボかどうかのフラグ

    public Combo(List<CardType> seq, float multiplier)
    {
        sequence = seq;
        damageMultiplier = multiplier;
        isOrderless = false;
    }
}

public class ComboManager
{
    private List<Combo> combos;            // 固有コンボ
    private List<Combo> genericCombos;     // 汎用コンボ

    public ComboManager(List<Combo> combosFromDeck, List<Combo> genericCombosFromDeck)
    {
        combos = combosFromDeck;
        genericCombos = genericCombosFromDeck;
    }

    /// <summary>
    /// 前のカードと今のカードを見てコンボが成立していたら倍率を返す
    /// 固有コンボ→汎用コンボの順で判定
    /// </summary>
    public float GetDamageMultiplier(List<Card> prevCards, Card currentCard, PlayerHP currentPlayer)
    {
        if (currentCard == null)
            return 1.0f;

        // 固有コンボ判定
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

        // 汎用コンボ判定
        float genericMultiplier = CheckGenericCombos(prevCards, currentCard);
        if (genericMultiplier > 1.0f)
        {
            // 汎用コンボには特殊効果なしで倍率だけ返す場合はこのまま返す
            return genericMultiplier;
        }

        return 1.0f;
    }

    /// <summary>
    /// 汎用コンボ判定（同じ手連続、シールド→攻撃など）
    /// </summary>
    private float CheckGenericCombos(List<Card> prevCards, Card currentCard)
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

    /// <summary>
    /// コンボ候補のリストを返す（UI表示用など）
    /// 固有コンボのみ判定
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
