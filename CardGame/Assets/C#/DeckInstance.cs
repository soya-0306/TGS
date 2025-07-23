using System.Collections.Generic;
using UnityEngine;

public class DeckInstance
{
    private List<Card> cards = new List<Card>();

    public DeckInstance(DeckDefinition def)
    {
        foreach (var entry in def.cards)  // entry ‚Í CardEntry
        {
            Sprite icon = LoadIcon(entry.cardType);
            string name = entry.cardType.ToString().ToLower();

            for (int i = 0; i < entry.count; i++)
            {
                cards.Add(new Card(entry.cardType, name, icon));
            }
        }
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int j = Random.Range(i, cards.Count);
            var tmp = cards[i];
            cards[i] = cards[j];
            cards[j] = tmp;
        }
    }

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

    private Sprite LoadIcon(CardType type)
    {
        return Resources.Load<Sprite>($"Sprites/{type.ToString().ToLower()}");
    }
}
