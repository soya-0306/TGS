using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedOrderUI : MonoBehaviour
{
    public Image[] cardImages;     // �\���X���b�g�i�ő�4�j
    public TextMeshProUGUI[] cardTexts; // ���O�\���i�C�Ӂj

    public void AddCardToSlot(int index, Card card)
    {
        if (index >= cardImages.Length) return;

        cardImages[index].sprite = card.Icon;
        cardImages[index].color = Color.white; // ��\���h�~

        if (cardTexts != null && index < cardTexts.Length)
        {
            cardTexts[index].text = card.Name;
        }
    }

/// <summary>
/// UI�������i���Z�b�g�j
/// </summary>
    //public void ClearAll()
    //{
    //    foreach (var slot in cardImages)
    //    {
    //        slot.Clear();
    //    }
    //}
}
