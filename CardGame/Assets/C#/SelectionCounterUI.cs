using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

public class SelectionCounterUI : MonoBehaviour
{
    public Image[] indicators;               // �������J�[�hUI�i�ÁE���ؑցj
    public Transform centerTarget;           // �J�[�h���ړ����钆���^�[�Q�b�g
    public CardSlotUI[] cardSlots;           // �\�����J�[�h�\���p�X���b�g�i���Ԃɕ\���j

    private Sprite darkSprite;
    private Sprite brightSprite;

    private Card[] selectedCards;

    private Vector3[] originalPositions;         // indicators�̌���localPosition
    private Vector3[] cardSlotsOriginalPositions; // cardSlots�̌���localPosition

    private Sprite transparentSprite;

    private Transform[] cardSlotsOriginalParents;  // ���̐e
    private Vector3[] cardSlotsOriginalScales;     // ���̃X�P�[��

    private Vector3[] originalScales; // �� new: indicators�̌���localScale

    void Awake()
    {
        darkSprite = Resources.Load<Sprite>("Sprites/uramen_dark");
        brightSprite = Resources.Load<Sprite>("Sprites/uramen_bright");

        if (darkSprite == null || brightSprite == null)
        {
            Debug.LogError("�X�v���C�g�̓ǂݍ��݂Ɏ��s���܂����B");
        }

        // indicators�̌��ʒu(localPosition)��ۑ�
        originalPositions = new Vector3[indicators.Length];
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] != null)
                originalPositions[i] = indicators[i].transform.localPosition;
        }

        // cardSlots�̌��ʒu(localPosition)��ۑ�
        cardSlotsOriginalPositions = new Vector3[cardSlots.Length];
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardSlots[i] != null)
                cardSlotsOriginalPositions[i] = cardSlots[i].transform.localPosition;
        }

        // �����̃X�v���C�g�ǂݍ��݂ɒǉ�
        transparentSprite = Resources.Load<Sprite>("Sprites/���߉摜");

        if (transparentSprite == null)
        {
            Debug.LogError("���߉摜�X�v���C�g�̓ǂݍ��݂Ɏ��s���܂����B�p�X�����������m�F���Ă��������B");
        }

        cardSlotsOriginalParents = new Transform[cardSlots.Length];
        cardSlotsOriginalScales = new Vector3[cardSlots.Length];

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardSlots[i] != null)
            {
                cardSlotsOriginalPositions[i] = cardSlots[i].transform.localPosition;
                cardSlotsOriginalParents[i] = cardSlots[i].transform.parent; // �� �e���L�^�I
                cardSlotsOriginalScales[i] = cardSlots[i].transform.localScale; // �� �X�P�[�����I
            }
        }

        originalScales = new Vector3[indicators.Length];
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] != null)
            {
                originalPositions[i] = indicators[i].transform.localPosition;
                originalScales[i] = indicators[i].transform.localScale; // �� �ǉ�
            }
        }

        ResetIndicators();
    }

    public void ResetIndicators()
    {
        UpdateSelectionCount(0);
    }

    public void UpdateSelectionCount(int count)
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] != null)
            {
                indicators[i].sprite = (i < count) ? brightSprite : darkSprite;
                indicators[i].color = Color.white;
            }
        }
    }

    /// <summary>
    /// �O������J�[�h�̔z����󂯎��i�J�[�h�I�����ɌĂԁj
    /// </summary>
    public void SetSelectedCards(Card[] cards)
    {
        selectedCards = cards;
    }

    /// <summary>
    /// ���J�[�h�𒆉��Ɉړ������A��\���ɂ��A�Ή�����\�J�[�h�𒆉����猳�̈ʒu�ɖ߂��A�j���[�V����
    /// </summary>
    // �@ �\����������i�ړ�������\�����\�J�[�h�����\���j����O�܂ł̏���
    public IEnumerator RevealCard(int index, Action onComplete = null)
    {
        if (selectedCards == null || index >= selectedCards.Length || index >= indicators.Length)
        {
            onComplete?.Invoke();
            yield break;
        }

        Image cardBack = indicators[index];
        if (cardBack.sprite != brightSprite)
        {
            onComplete?.Invoke();
            yield break;
        }

        cardBack.transform.SetAsLastSibling();

        // ���̃X�P�[����ۑ��i����Ȃ����1f�Œ�ł�OK�j
        Vector3 originalScale = cardBack.transform.localScale;

        // �����Ɉړ����Ȃ���X�P�[���A�b�v
        yield return DOTween.Sequence()
            .Join(cardBack.transform.DOMove(centerTarget.position, 0.5f))
            .Join(cardBack.transform.DOScale(originalScale * 1.5f, 0.5f))
            .WaitForCompletion();

        // ��]�i�߂��艉�o�j
        yield return cardBack.transform.DORotate(new Vector3(0, 90, 0), 0.25f).WaitForCompletion();

        // ���J�[�h��\���ɂ��ĕ\�J�[�h�\��
        cardBack.gameObject.SetActive(false);

        if (index < cardSlots.Length && cardSlots[index] != null)
        {
            var slot = cardSlots[index];

            slot.transform.SetParent(centerTarget, false);
            slot.transform.localPosition = Vector3.zero;
            slot.transform.localRotation = Quaternion.Euler(0, 90, 0);
            slot.SetCard(selectedCards[index]);
            slot.gameObject.SetActive(true);

            // �\�J�[�h���X�P�[���A�b�v������Ԃ���X�^�[�g
            slot.transform.localScale = originalScale * 1.5f;

            // ��]���Đ��ʂ�
            yield return slot.transform.DOLocalRotate(Vector3.zero, 0.25f).WaitForCompletion();
        }

        onComplete?.Invoke();
    }

    public IEnumerator ReturnRevealedCard(int index, Action onComplete = null)
    {
        if (index < cardSlots.Length && cardSlots[index] != null)
        {
            var slot = cardSlots[index];

            // 1. �e�����ɖ߂��i�Y����h���j
            slot.transform.SetParent(cardSlotsOriginalParents[index], false);

            // 2. �X�P�[���ƈʒu�����ɖ߂�
            yield return DOTween.Sequence()
                .Join(slot.transform.DOLocalMove(cardSlotsOriginalPositions[index], 0.5f).SetEase(Ease.OutQuad))
                .Join(slot.transform.DOScale(cardSlotsOriginalScales[index], 0.5f).SetEase(Ease.OutQuad))
                .WaitForCompletion();
        }

        onComplete?.Invoke();
    }

    public void ShowAllChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public IEnumerator ResetCardsPosition()
    {
        int completed = 0;
        int total = indicators.Length;

        for (int i = 0; i < total; i++)
        {
            StartCoroutine(MoveCardBackToOriginal(i, () => { completed++; }));
        }

        yield return new WaitUntil(() => completed >= total);
    }

    private IEnumerator MoveCardBackToOriginal(int index, Action onComplete)
    {
        if (index < 0 || index >= indicators.Length)
        {
            onComplete?.Invoke();
            yield break;
        }

        var cardTransform = indicators[index].transform;
        Vector3 originalPos = originalPositions[index];

        yield return cardTransform.DOLocalMove(originalPos, 0.5f).SetEase(Ease.OutQuad).WaitForCompletion();
        yield return cardTransform.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.OutQuad).WaitForCompletion();

        if (!cardTransform.gameObject.activeSelf)
            cardTransform.gameObject.SetActive(true);

        onComplete?.Invoke();
    }

    public void ResetCardSlots()
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardSlots[i] != null)
            {
                cardSlots[i].SetCard(null); // ���O��A�C�R�������Z�b�g

                if (transparentSprite != null)
                    cardSlots[i].iconImage.sprite = transparentSprite;

                cardSlots[i].gameObject.SetActive(false); // �\�J�[�hUI���\����
            }
        }
    }

    public GameObject GetCardObject(int index)
    {
        if (index >= 0 && index < cardSlots.Length && cardSlots[index] != null)
        {
            return cardSlots[index].gameObject;
        }
        return null;
    }

    public void ResetCardScales()
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] != null)
            {
                indicators[i].transform.localScale = originalScales[i]; // �� ���X�P�[���ɖ߂�
            }
        }
    }

}
