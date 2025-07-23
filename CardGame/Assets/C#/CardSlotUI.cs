using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// �o�����Ԃ̃J�[�h1��\������UI�X���b�g
/// </summary>
public class CardSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;

    /// <summary>
    /// �X���b�g�ɃJ�[�h��ݒ�
    /// </summary>
    public void SetCard(Card card)
    {
        if (card == null)
        {
            Clear(); // �� null �̂Ƃ��̓X���b�g����ɂ���
            return;
        }

        iconImage.sprite = card.Icon;
        nameText.text = card.Name;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// �X���b�g����ɂ���
    /// </summary>
    public void Clear()
    {
        iconImage.sprite = null;
        nameText.text = "";
        gameObject.SetActive(false);
    }
}