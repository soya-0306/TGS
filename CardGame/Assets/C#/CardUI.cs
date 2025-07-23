using UnityEngine;
using UnityEngine.UI;
using TMPro; // �� �����ǉ�

/// <summary>
/// 1���̃J�[�hUI���Ǘ�����N���X�i�\���ƃf�[�^�̕R�t���j
/// </summary>
public class CardUI : MonoBehaviour
{
    [Header("UI�Q��")]
    public Image iconImage;     // �J�[�h�̃A�C�R���iImage�R���|�[�l���g�j
    public TMP_Text nameText;       // �J�[�h�̖��O�iText�R���|�[�l���g�j

    private Card cardData;      // �\���Ώۂ̃J�[�h�f�[�^

    /// <summary>
    /// �\������J�[�h�̃f�[�^��ݒ�
    /// </summary>
    public void SetCard(Card card)
    {
        cardData = card;

        // UI���X�V
        if (iconImage != null)
        {
            iconImage.sprite = card.Icon;
        }

        if (nameText != null)
        {
            nameText.text = card.Name;
        }
    }

    /// <summary>
    /// ����UI�J�[�h�������Ă���J�[�h�f�[�^��Ԃ�
    /// </summary>
    public Card GetCard()
    {
        return cardData;
    }
}
