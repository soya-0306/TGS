using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

public class SelectionCounterUI : MonoBehaviour
{
    public Image[] indicators;               // 裏向きカードUI（暗・明切替）
    public Transform centerTarget;           // カードを移動する中央ターゲット
    public CardSlotUI[] cardSlots;           // 表向きカード表示用スロット（順番に表示）

    private Sprite darkSprite;
    private Sprite brightSprite;

    private Card[] selectedCards;

    private Vector3[] originalPositions;         // indicatorsの元のlocalPosition
    private Vector3[] cardSlotsOriginalPositions; // cardSlotsの元のlocalPosition

    private Sprite transparentSprite;

    private Transform[] cardSlotsOriginalParents;  // 元の親
    private Vector3[] cardSlotsOriginalScales;     // 元のスケール

    private Vector3[] originalScales; // ← new: indicatorsの元のlocalScale

    void Awake()
    {
        darkSprite = Resources.Load<Sprite>("Sprites/uramen_dark");
        brightSprite = Resources.Load<Sprite>("Sprites/uramen_bright");

        if (darkSprite == null || brightSprite == null)
        {
            Debug.LogError("スプライトの読み込みに失敗しました。");
        }

        // indicatorsの元位置(localPosition)を保存
        originalPositions = new Vector3[indicators.Length];
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] != null)
                originalPositions[i] = indicators[i].transform.localPosition;
        }

        // cardSlotsの元位置(localPosition)を保存
        cardSlotsOriginalPositions = new Vector3[cardSlots.Length];
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardSlots[i] != null)
                cardSlotsOriginalPositions[i] = cardSlots[i].transform.localPosition;
        }

        // 既存のスプライト読み込みに追加
        transparentSprite = Resources.Load<Sprite>("Sprites/透過画像");

        if (transparentSprite == null)
        {
            Debug.LogError("透過画像スプライトの読み込みに失敗しました。パスが正しいか確認してください。");
        }

        cardSlotsOriginalParents = new Transform[cardSlots.Length];
        cardSlotsOriginalScales = new Vector3[cardSlots.Length];

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardSlots[i] != null)
            {
                cardSlotsOriginalPositions[i] = cardSlots[i].transform.localPosition;
                cardSlotsOriginalParents[i] = cardSlots[i].transform.parent; // ← 親を記録！
                cardSlotsOriginalScales[i] = cardSlots[i].transform.localScale; // ← スケールも！
            }
        }

        originalScales = new Vector3[indicators.Length];
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] != null)
            {
                originalPositions[i] = indicators[i].transform.localPosition;
                originalScales[i] = indicators[i].transform.localScale; // ← 追加
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
    /// 外部からカードの配列を受け取る（カード選択時に呼ぶ）
    /// </summary>
    public void SetSelectedCards(Card[] cards)
    {
        selectedCards = cards;
    }

    /// <summary>
    /// 裏カードを中央に移動させ、非表示にし、対応する表カードを中央から元の位置に戻すアニメーション
    /// </summary>
    // ① 表示だけする（移動→裏非表示→表カード中央表示）判定前までの処理
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

        // 元のスケールを保存（いらなければ1f固定でもOK）
        Vector3 originalScale = cardBack.transform.localScale;

        // 中央に移動しながらスケールアップ
        yield return DOTween.Sequence()
            .Join(cardBack.transform.DOMove(centerTarget.position, 0.5f))
            .Join(cardBack.transform.DOScale(originalScale * 1.5f, 0.5f))
            .WaitForCompletion();

        // 回転（めくり演出）
        yield return cardBack.transform.DORotate(new Vector3(0, 90, 0), 0.25f).WaitForCompletion();

        // 裏カード非表示にして表カード表示
        cardBack.gameObject.SetActive(false);

        if (index < cardSlots.Length && cardSlots[index] != null)
        {
            var slot = cardSlots[index];

            slot.transform.SetParent(centerTarget, false);
            slot.transform.localPosition = Vector3.zero;
            slot.transform.localRotation = Quaternion.Euler(0, 90, 0);
            slot.SetCard(selectedCards[index]);
            slot.gameObject.SetActive(true);

            // 表カードもスケールアップした状態からスタート
            slot.transform.localScale = originalScale * 1.5f;

            // 回転して正面に
            yield return slot.transform.DOLocalRotate(Vector3.zero, 0.25f).WaitForCompletion();
        }

        onComplete?.Invoke();
    }

    public IEnumerator ReturnRevealedCard(int index, Action onComplete = null)
    {
        if (index < cardSlots.Length && cardSlots[index] != null)
        {
            var slot = cardSlots[index];

            // 1. 親を元に戻す（ズレを防ぐ）
            slot.transform.SetParent(cardSlotsOriginalParents[index], false);

            // 2. スケールと位置を元に戻す
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
                cardSlots[i].SetCard(null); // 名前やアイコンをリセット

                if (transparentSprite != null)
                    cardSlots[i].iconImage.sprite = transparentSprite;

                cardSlots[i].gameObject.SetActive(false); // 表カードUIを非表示に
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
                indicators[i].transform.localScale = originalScales[i]; // ← 元スケールに戻す
            }
        }
    }

}
