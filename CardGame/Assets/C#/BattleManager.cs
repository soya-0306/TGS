using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using static Combo;
using System.Collections;
using static SelectionCounterUI;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ゲームの進行（フェーズ管理・じゃんけん処理・HP管理・コンボ処理）を司るメインクラス
public class BattleManager : MonoBehaviour
{
    DirectionalCardSelector directionalCardSelector;

    // 各プレイヤーのカード選択ロジックを扱うコンポーネント
    public DirectionalCardSelector player1Selector;
    public DirectionalCardSelector player2Selector;

    // 手札のUI表示用オブジェクト
    public GameObject player1CardDisplay;
    public GameObject player2CardDisplay;

    // 手札を表示するスロット（UI）への参照
    public CardSlotUI[] player1CardSlots;
    public CardSlotUI[] player2CardSlots;

    // カードを順番に公開していくフェーズのUIパネル
    public GameObject player1CardShowPanel;
    public GameObject player2CardShowPanel;

    // プレイヤーのカード選択結果を保持(コンボに使用)
    private Dictionary<int, List<Card>> playerSelections = new Dictionary<int, List<Card>>();

    // ラウンドと最終結果を表示するUIテキスト
    public TMP_Text roundResultText;
    public TMP_Text finalResultText;

    // 勝ち数カウント
    private int player1WinCount = 0;
    private int player2WinCount = 0;

    public GameObject resultTextUIObject;
    public TMP_Text resultText;

    public GameObject roundResultTextUIObject; // ← Round の結果表示UIの親オブジェクト

    // HPを扱うコンポーネント
    public PlayerHP player1HP;
    public PlayerHP player2HP;

    // ラウンド管理
    private int currentRound = 1;
    private int totalRounds = 3;

    //ComboManager comboManager = new ComboManager();

    // プレイヤーが選択したデッキ情報（デッキごとの固有コンボ含む）
    public DeckDefinition player1Deck;
    public DeckDefinition player2Deck;

    private ComboManager player1ComboManager;
    private ComboManager player2ComboManager;

    public Card currentCard;
    public Card playerCurrentCard;
    public Card opponentCurrentCard;

    // ダメージを受けた際に一時的に赤く光るUIパネル
    public CanvasGroup player1DamageColorPanelCanvasGroup;
    public CanvasGroup player2DamageColorPanelCanvasGroup;

    public SelectionCounterUI player1RevealUI;
    public SelectionCounterUI player2RevealUI;

    [Header("エフェクト")]
    public GameObject drawEffectPrefab; // あいこ用
    public GameObject fireEffectPrefab;

    public Transform drawEffectCenterPoint;

    [Header("キャラクター召喚用の設定")]
    public Transform player1SpawnPoint;
    public Transform player2SpawnPoint;

    private GameObject player1Character;
    private GameObject player2Character;

    [Header("選択フェーズの制限時間UI")]
    public float selectionTimeLimit = 60f;
    private float selectionTimer = 0f;
    private bool isSelectionTimerRunning = false;

    [Header("リロールフェーズの制限時間UI")]
    private float rerollTimer = 0f;
    private float rerollTimeLimit = 10f;
    private bool isRerollTimerRunning = false;
    private bool p1WantsReroll = false;
    private bool p2WantsReroll = false;

    public TMP_Text selectionTimerText;

    [Header("ダメージ吹き出し")]
    public GameObject damagePopupPrefab; // 吹き出しプレハブ
    public Vector3 damagePopupOffset = new Vector3(0f, 100f, 0f); // カードからのオフセット位置
    private int lastDamageAmount = 0; // 最後に発生したダメージ

    [Header("コンボリスト")]
    public Transform player1ComboListParent;  // 1P用のUI親
    public Transform player2ComboListParent;  // 2P用のUI親

    public GameObject comboListUIPrefab;  // Imageコンポを持ったPrefab

    private GameObject player1ComboListUIInstance;
    private GameObject player2ComboListUIInstance;

    [Header("帯")]
    [SerializeField] private GameObject bannerObject; // 帯本体のオブジェクト（Image付き）
    [SerializeField] private RectTransform bannerRectTransform; // アニメ用RectTransform
    [SerializeField] private Image bannerImage; // 表示する画像を差し替える

    [SerializeField] private Sprite battleStartSprite;
    [SerializeField] private Sprite round1Sprite;
    [SerializeField] private Sprite round2Sprite;
    [SerializeField] private Sprite round3Sprite;

    [SerializeField] private float bannerSlideDuration = 0.5f;
    [SerializeField] private float bannerWaitTime = 1.2f;


    public AudioClip damageSound;  // 体力が減ったときの音（後からインスペクターでセット）
    private AudioSource audioSource;

    public BattleSEPlayer sePlayer;

    void Start()
    {
        // 各プレイヤーにIDを割り当て
        player1Selector.playerId = 1;
        player2Selector.playerId = 2;

        // デッキ情報を取得（DeckSelector から取得）
        player1Deck = DeckSelector.Instance.player1Deck;
        player2Deck = DeckSelector.Instance.player2Deck;

        // デッキ情報を各Selectorに渡す
        player1Selector.playerDeckDefinition = player1Deck;
        player2Selector.playerDeckDefinition = player2Deck;

        // 各UIの初期状態を設定
        if (player1CardShowPanel != null) player1CardShowPanel.SetActive(false);
        if (player2CardShowPanel != null) player2CardShowPanel.SetActive(false);

        player1Selector.OnSelectionComplete += OnPlayerSelectionComplete;
        player2Selector.OnSelectionComplete += OnPlayerSelectionComplete;

        if (resultTextUIObject != null) resultTextUIObject.SetActive(false);
        if (roundResultText != null) roundResultText.text = "";
        if (finalResultText != null) finalResultText.text = "";

        if (roundResultTextUIObject != null) roundResultTextUIObject.SetActive(false);

        // 汎用コンボをデッキから取得してComboManagerに渡す
        List<Combo> genericCombosFromDeck = player1Deck.genericCombos; // player1DeckのgenericCombos
        player1ComboManager = new ComboManager(player1Deck.combos, genericCombosFromDeck);

        genericCombosFromDeck = player2Deck.genericCombos; // player2DeckのgenericCombos
        player2ComboManager = new ComboManager(player2Deck.combos, genericCombosFromDeck);

        // ▼ プレイヤーキャラの召喚
        if (player1Deck.deckCharacterPrefab != null && player1SpawnPoint != null)
        {
            player1Character = Instantiate(player1Deck.deckCharacterPrefab, player1SpawnPoint);
            player1Character.transform.localPosition = Vector3.zero;
            player1Character.transform.localRotation = Quaternion.identity;
            player1Character.transform.localScale = Vector3.one;
        }

        if (player2Deck.deckCharacterPrefab != null && player2SpawnPoint != null)
        {
            player2Character = Instantiate(player2Deck.deckCharacterPrefab, player2SpawnPoint);
            player2Character.transform.localPosition = Vector3.zero;
            player2Character.transform.localRotation = Quaternion.identity;
            player2Character.transform.localScale = Vector3.one;
        }

        StartCoroutine(StartBattleSequence());

        //StartRerollPhase();
    }

    void Update()
    {
        if (currentRound > totalRounds)
        {
            // XBOXコントローラーのBボタンは joystick button 0
            if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.B))
            {
                ReturnToTitle();
            }
        }

        // ▼ リロールフェーズの制限時間処理
        if (isRerollTimerRunning)
        {
            rerollTimer += Time.deltaTime;
            float remaining = Mathf.Clamp(rerollTimeLimit - rerollTimer, 0f, rerollTimeLimit);

            if (selectionTimerText != null)
            {
                selectionTimerText.text = $"リロール残り時間: {Mathf.CeilToInt(remaining)}秒";
            }

            //リロールはXボタン joystick button 2
            if (Input.GetKeyDown(KeyCode.R)) p1WantsReroll = true;
            if (Input.GetKeyDown(KeyCode.Joystick1Button2)) p1WantsReroll = true;
            if (Input.GetKeyDown(KeyCode.Return)) p2WantsReroll = true;
            if (Input.GetKeyDown(KeyCode.Joystick2Button2)) p2WantsReroll = true;

            if (rerollTimer >= rerollTimeLimit)
            {
                isRerollTimerRunning = false;
                EnterRerollSelectionPhase();
            }

            // ✅ 注意：リロール中は選択タイマー動かさない
            return;
        }

        // ▼ 選択フェーズの制限時間処理
        if (isSelectionTimerRunning)
        {
            selectionTimer += Time.deltaTime;

            float remaining = Mathf.Clamp(selectionTimeLimit - selectionTimer, 0f, selectionTimeLimit);
            if (selectionTimerText != null)
            {
                selectionTimerText.text = $"選択残り時間: {Mathf.CeilToInt(remaining)}秒";
            }

            if (selectionTimer >= selectionTimeLimit)
            {
                isSelectionTimerRunning = false;
                ForceSelectRemainingCards();
            }
        }

        // Qキーで1Pのコンボリスト表示切替
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            ToggleComboListUI(1);
        }

        // Deleteキーで2Pのコンボリスト表示切替
        if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Joystick2Button3))
        {
            ToggleComboListUI(2);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            FadeManager.Instance.LoadScene("ResultScene", 2.0f);
        }
    }

    // プレイヤーがカード選択を終えた時に呼ばれる
    void OnPlayerSelectionComplete(int playerId, List<Card> selectedCards)
    {
        if (!playerSelections.ContainsKey(playerId))
            playerSelections[playerId] = new List<Card>(selectedCards);

        // カード公開用にセット
        if (playerId == 1)
            player1RevealUI?.SetSelectedCards(selectedCards.ToArray());
        else if (playerId == 2)
            player2RevealUI?.SetSelectedCards(selectedCards.ToArray());

        if (playerSelections.Count == 2)
        {
            player1Selector.gameObject.SetActive(false);
            player2Selector.gameObject.SetActive(false);

            if (player1CardDisplay != null) player1CardDisplay.SetActive(false);
            if (player2CardDisplay != null) player2CardDisplay.SetActive(false);

            StartCoroutine(RevealCardsThenStartBattle());
        }
    }

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // バトルフェーズ：1ラウンド4回のじゃんけんを実行
    IEnumerator BattlePhase()
    {
        if (player1CardShowPanel != null) player1CardShowPanel.SetActive(true);
        if (player2CardShowPanel != null) player2CardShowPanel.SetActive(true);
        if (roundResultTextUIObject != null) roundResultTextUIObject.SetActive(true);

        List<Card> p1Cards = playerSelections[1];
        List<Card> p2Cards = playerSelections[2];

        player1WinCount = 0;
        player2WinCount = 0;

        List<Card> p1History = new List<Card>();
        List<Card> p2History = new List<Card>();

        for (int i = 0; i < 4; i++)
        {
            bool p1Done = false;
            bool p2Done = false;

            StartCoroutine(player1RevealUI.RevealCard(i, () => p1Done = true));
            StartCoroutine(player2RevealUI.RevealCard(i, () => p2Done = true));
            yield return new WaitUntil(() => p1Done && p2Done);

            yield return new WaitForSeconds(1f);

            Card c1 = p1Cards[i];
            Card c2 = p2Cards[i];

            string[] ordinals = { "1st", "2nd", "3rd", "4th" };
            string roundResult = $"{ordinals[i]} Card";

            bool skipJudgement = false;

            float p1Multiplier = 1f;
            float p2Multiplier = 1f;

            GameObject p1CardObj = player1RevealUI.GetCardObject(i);
            GameObject p2CardObj = player2RevealUI.GetCardObject(i);
            
            int damagedPlayer = 0;

            // 両者シールド → コンボキャンセルだけして判定スキップ
            if (c1.IsShield() && c2.IsShield())
            {
                sePlayer.PlayTieOrShieldSE();
                roundResult += " :Combo Cancel!";
                p1History.Clear();
                p2History.Clear();
                skipJudgement = true;
                yield return StartCoroutine(DoDrawEffect(p1CardObj, p2CardObj));
            }
            else
            {
                p1Multiplier = player1ComboManager.GetDamageMultiplier(p1History, c1, player1HP);
                p2Multiplier = player2ComboManager.GetDamageMultiplier(p2History, c2, player2HP);
            }

            if (!skipJudgement)
            {
                if (c1.IsShield())
                {
                    //roundResult += "P1 Shield - No damage";
                    yield return StartCoroutine(DoDrawEffect(p1CardObj, p2CardObj));
                    sePlayer.PlayTieOrShieldSE();
                }
                else if (c2.IsShield())
                {
                    yield return StartCoroutine(DoDrawEffect(p1CardObj, p2CardObj));
                    sePlayer.PlayTieOrShieldSE();
                    //roundResult += "P2 Shield - No damage";
                }
                else if (c1.Beats(c2))
                {
                    int dmg = Mathf.CeilToInt(1 * p1Multiplier);
                    lastDamageAmount = dmg;
                    player2HP.TakeDamage(dmg);

                    //roundResult += $"Player2 →{dmg} Damage";
                    //player2HP.TakeDamage(dmg);

                    player1WinCount++;
                    damagedPlayer = 2;
                    // ダメージパネルの表示をDoChargeAttack内に移動したのでここでは呼ばない
                    yield return StartCoroutine(DoChargeAttack(p1CardObj, p2CardObj, damagedPlayer));
                    //// Player1が勝った場合
                    //PlayCharacterAnimation(player1Character, c1.Type);

                    // 勝者のアニメーションと敗者のリアクションを一括で再生
                    PlayBattleAnimations(player1Character, c1.Type, player2Character);

                    sePlayer.PlayDamageSE();
                    
                    // 音を鳴らす
                    if (damageSound != null)
                    {
                        audioSource.PlayOneShot(damageSound);
                        Debug.Log("🔊 プレイヤー1が勝利、ダメージ音再生");
                    }
                    else
                    {
                        Debug.Log("⚠️ damageSoundが未設定のため、音は再生されませんでした。");
                    }
                }
                else if (c2.Beats(c1))
                {
                    int dmg = Mathf.CeilToInt(1 * p2Multiplier);
                    lastDamageAmount = dmg;
                    player1HP.TakeDamage(dmg);

                    //roundResult += $"Player1 →{dmg} Damage";
                    //player1HP.TakeDamage(dmg);

                    player2WinCount++;
                    damagedPlayer = 1;
                    // ダメージパネルの表示をDoChargeAttack内に移動したのでここでは呼ばない
                    yield return StartCoroutine(DoChargeAttack(p2CardObj, p1CardObj, damagedPlayer));
                    //// Player2が勝った場合
                    //PlayCharacterAnimation(player2Character, c2.Type);

                    // 勝者のアニメーションと敗者のリアクションを一括で再生
                    PlayBattleAnimations(player2Character, c2.Type, player1Character);

                    sePlayer.PlayDamageSE();

                    // 音を鳴らす
                    if (damageSound != null)
                    {
                        audioSource.PlayOneShot(damageSound);
                        Debug.Log("🔊 プレイヤー2が勝利、ダメージ音再生");
                    }
                    else
                    {
                        Debug.Log("⚠️ damageSoundが未設定のため、音は再生されませんでした。");
                    }
                }
                else
                {
                    roundResult += "Draw";
                    yield return StartCoroutine(DoDrawEffect(p1CardObj, p2CardObj));
                    sePlayer.PlayTieOrShieldSE();
                }
            }

            roundResultText.text = roundResult;

            // 履歴更新
            // 両者シールドの場合は履歴クリア済みなのでカードを追加しない
            if (!(c1.IsShield() && c2.IsShield()))
            {
                p1History.Add(c1);
                p2History.Add(c2);
            }

            yield return new WaitForSeconds(1f);

            // ▼ このアニメーションはスキップさせない！！
            bool p1Back = false;
            bool p2Back = false;

            StartCoroutine(player1RevealUI.ReturnRevealedCard(i, () => p1Back = true));
            StartCoroutine(player2RevealUI.ReturnRevealedCard(i, () => p2Back = true));
            yield return new WaitUntil(() => p1Back && p2Back);

            yield return new WaitForSeconds(1f);

            // 毎ターンの最後にHPチェック（即終了処理）
            if (player1HP.currentHP <= 0 || player2HP.currentHP <= 0)
            {
                Debug.Log("Player HP reached 0. Ending round immediately.");
                yield return EndOfRound(totalRounds);  // リザルト処理に進む
                yield break;                // これ以上処理をしない（BattlePhase即終了）
            }
        }

        yield return EndOfRound(totalRounds);
    }

    IEnumerator EndOfRound(int roundNumber)
    {
        Debug.Log("Battle Finished!");

        if (resultTextUIObject != null) resultTextUIObject.SetActive(true);

        Sprite roundSprite = null;
        switch (roundNumber)
        {
            case 2:
                roundSprite = round2Sprite;
                break;
            case 3:
                roundSprite = round3Sprite;
                break;
        }

        if (roundSprite != null)
        {
            yield return PlayBannerAnimation(roundSprite);
        }

        //yield return new WaitForSeconds(3f);

        currentRound++;

        // ▼ HPが0のプレイヤーがいるなら強制終了
        if (player1HP.currentHP <= 0 || player2HP.currentHP <= 0 || currentRound > totalRounds)
        {
            string resultTextStr = "";
            currentRound++;

            if (player1HP.currentHP > player2HP.currentHP)
                resultTextStr = "Player 1 の勝利！\nBボタンでタイトルに戻る";
            else if (player2HP.currentHP > player1HP.currentHP)
                resultTextStr = "Player 2 の勝利！\nBボタンでタイトルに戻る";
            else
                resultTextStr = "引き分け！\nBボタンでタイトルに戻る";

            // GameResultManager にテキスト保存（ResultSceneで使う）
            if (GameResultManager.Instance != null)
            {
                GameResultManager.Instance.finalResultText = resultTextStr;
            }

            yield return new WaitForSeconds(2f); // 少し待ってから遷移

            // リザルト画面に移動
            FadeManager.Instance.LoadScene("ResultScene", 2.0f);

            yield break; // 処理終了（次ラウンドに行かない）
        }
        else
        {
            player1CardShowPanel.SetActive(false);
            player2CardShowPanel.SetActive(false);
            roundResultTextUIObject.SetActive(false);
            resultTextUIObject.SetActive(false);

            playerSelections.Clear();

            player1Selector.RefreshHand();
            player2Selector.RefreshHand();

            player1Selector.ResetSelector();
            player2Selector.ResetSelector();

            player1Selector.gameObject.SetActive(true);
            player2Selector.gameObject.SetActive(true);

            player1CardDisplay.SetActive(true);
            player2CardDisplay.SetActive(true);

            player1RevealUI.gameObject.SetActive(true);
            player2RevealUI.gameObject.SetActive(true);

            selectionTimerText.gameObject.SetActive(true);

            player1RevealUI.ShowAllChildren();
            player2RevealUI.ShowAllChildren();

            player1RevealUI.ResetCardScales();  
            player2RevealUI.ResetCardScales();  

            // ▼ カードUIを透過でリセット
            player1RevealUI.ResetCardSlots();
            player2RevealUI.ResetCardSlots();

            // 裏向きカードUIも元位置に戻す例
            StartCoroutine(player1RevealUI.ResetCardsPosition());
            StartCoroutine(player2RevealUI.ResetCardsPosition());

            roundResultText.text = $"Round {currentRound} Start!";
        }
        StartRerollPhase();
    }

    //ダメージを受けた側が一瞬だけ赤いパネルが出てくる
    private void ShowDamagePanel(int playerId)
    {
        if (playerId == 1)
        {
            player1DamageColorPanelCanvasGroup.gameObject.SetActive(true);
            player1DamageColorPanelCanvasGroup.alpha = 1f;
            StartCoroutine(FadeOutPanel(player1DamageColorPanelCanvasGroup, 1f)); // 1秒かけてフェードアウト
        }
        else if (playerId == 2)
        {
            player2DamageColorPanelCanvasGroup.gameObject.SetActive(true);
            player2DamageColorPanelCanvasGroup.alpha = 1f;
            StartCoroutine(FadeOutPanel(player2DamageColorPanelCanvasGroup, 1f));
        }
    }

    private IEnumerator FadeOutPanel(CanvasGroup panelCanvasGroup, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator DoChargeAttack(GameObject attacker, GameObject target, int damagedPlayer)
    {
        Vector3 originalPos = attacker.transform.position;
        Vector3 targetPos = target.transform.position;
        Vector3 direction;

        if (damagedPlayer == 1)
        {
            // プレイヤー1がダメージ受けた → プレイヤー2が攻撃してるから左向きに攻撃
            direction = (targetPos - originalPos).normalized;
        }
        else if (damagedPlayer == 2)
        {
            // プレイヤー2がダメージ受けた → プレイヤー1が攻撃してるから右向きに攻撃
            direction = (targetPos - originalPos).normalized;
        }
        else
        {
            direction = (targetPos - originalPos).normalized;
        }

        // プレイヤーによって移動方向を反転させる例
        if (damagedPlayer == 1)
        {
            // 2Pが攻撃者 → 右から左に動く
            direction = Vector3.left;
        }
        else if (damagedPlayer == 2)
        {
            // 1Pが攻撃者 → 左から右に動く
            direction = Vector3.right;
        }

        Vector3 hitPoint = originalPos + direction * 20f;

        yield return attacker.transform.DOMove(hitPoint, 0.3f).SetEase(Ease.OutQuad).WaitForCompletion();

        ShowDamagePanel(damagedPlayer);
        ShowDamageEffect(target);

        yield return new WaitForSeconds(0.2f);

        yield return attacker.transform.DOMove(originalPos, 0.3f).SetEase(Ease.InQuad).WaitForCompletion();
    }

    void ShowDamageEffect(GameObject cardObj)
    {
        var effect = Instantiate(fireEffectPrefab, cardObj.transform);
        effect.transform.localPosition = new Vector3(0f, -30f, 0f);
        effect.transform.localScale = Vector3.one * 30f;
        Destroy(effect, 0.6f); //自動破棄

        // ダメージ値を取得（これは直前の BattlePhase で渡す必要がある）
        int damage = lastDamageAmount; // 後述する変数で保持する
        ShowDamagePopup(cardObj, damage);
    }


    //private IEnumerator HideDamagePanelAfterDelay(GameObject panel)
    //{
    //    yield return new WaitForSeconds(1.0f);
    //    panel.SetActive(false);
    //}

    private void ShowCardToSlot(GameObject panel, int index, Card card)
    {
        var slots = panel.GetComponentsInChildren<CardSlotUI>();
        if (index >= 0 && index < slots.Length)
        {
            slots[index].SetCard(card);
        }
    }

    // ランダムにカードを差し替える処理
    private void ReplaceOpponentCardRandomly(Deck opponentDeck)
    {
        Card newCard = opponentDeck.DrawCards(1)[0]; // ← DrawRandomCard() じゃなく DrawCards(1) を使う場合
        if (newCard != null)
        {
            opponentCurrentCard = newCard; // BattleManager のフィールド（相手の現在のカード）
            Debug.Log($"相手のカードをランダムに差し替えました: {newCard.Name}");
        }
    }

    // コンボ発動時に呼ばれる処理
    public void OnComboTriggered(int comboEffectID, PlayerHP player, PlayerHP opponent, Deck opponentDeck)
    {
        ComboEffectManager.ApplyEffect(comboEffectID, player);
        bool needRandomReplace = ComboEffectManager.IsRandomReplaceNeeded(comboEffectID);

        if (needRandomReplace)
        {
            ReplaceOpponentCardRandomly(opponentDeck);
            Debug.Log("ランダム差し替え後、もう一度じゃんけんを実行");
            // ここで JudgeHand(playerCurrentCard, opponentCurrentCard); を呼んでもいい
        }
    }

    private IEnumerator RevealCardsThenStartBattle()
    {
         // ▼ ここでタイマーを消す
        if (selectionTimerText != null)
        {
            selectionTimerText.gameObject.SetActive(false);
        }

        // 4枚を順に処理しつつ、1枚ずつ同時に中央に出すBattlePhaseを呼ぶ
        yield return StartCoroutine(BattlePhase());
    }

    public void ReturnToTitle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }

    //private void PlayCharacterAnimation(GameObject character, CardType cardType)
    //{
    //    if (character == null) return;

    //    var animController = character.GetComponent<KeyAnimationController>();
    //    if (animController == null) return;

    //    animController.PlayCardAnimation(cardType); // 新しく作るメソッド
    //    animController.PlayDamageAnimation();
    //}

    private void PlayBattleAnimations(GameObject winnerCharacter, CardType winnerCardType, GameObject loserCharacter)
    {
        // 攻撃アニメーション
        if (winnerCharacter != null)
        {
            var winnerAnim = winnerCharacter.GetComponent<KeyAnimationController>();
            if (winnerAnim != null)
            {
                winnerAnim.PlayCardAnimation(winnerCardType);
            }
        }

        // ダメージアニメーション
        if (loserCharacter != null)
        {
            var loserAnim = loserCharacter.GetComponent<KeyAnimationController>();
            if (loserAnim != null)
            {
                loserAnim.PlayDamageAnimation();
            }
        }
    }


    private IEnumerator DoDrawEffect(GameObject card1, GameObject card2)
    {
        float moveDuration = 0.3f;

        // カード1 & 2 を中央に移動
        Vector3 centerPos = drawEffectCenterPoint.position;

        Tween move1 = card1.transform.DOMove(centerPos + Vector3.left * 3f, moveDuration).SetEase(Ease.OutQuad);
        Tween move2 = card2.transform.DOMove(centerPos + Vector3.left * -3f, moveDuration).SetEase(Ease.OutQuad);

        yield return DOTween.Sequence().Join(move1).Join(move2).WaitForCompletion();

        // 衝突後、エフェクト表示
        if (drawEffectPrefab != null)
        {
            GameObject effect = Instantiate(drawEffectPrefab, drawEffectCenterPoint.position, Quaternion.identity, drawEffectCenterPoint);
            effect.transform.localScale = Vector3.one * 100f;
            Destroy(effect, 0.6f);
        }

        yield return new WaitForSeconds(0.3f);

        // 元に戻す
        Tween return1 = card1.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.InQuad);
        Tween return2 = card2.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.InQuad);

        yield return DOTween.Sequence().Join(return1).Join(return2).WaitForCompletion();
    }

    private void ForceCompleteSelections()
    {
        if (!playerSelections.ContainsKey(1))
        {
            List<Card> autoCards = player1Selector.ForceSelectRemainingCards();
            OnPlayerSelectionComplete(1, autoCards);
        }

        if (!playerSelections.ContainsKey(2))
        {
            List<Card> autoCards = player2Selector.ForceSelectRemainingCards();
            OnPlayerSelectionComplete(2, autoCards);
        }
    }

    // ラウンドの開始時などに呼び出す
    private void StartSelectionPhase()
    {
        player1Selector.gameObject.SetActive(true);
        player2Selector.gameObject.SetActive(true);
        player1CardDisplay.SetActive(true);
        player2CardDisplay.SetActive(true);

        selectionTimer = 0f;
        isSelectionTimerRunning = true;

        if (selectionTimerText != null)
        {
            selectionTimerText.gameObject.SetActive(true); // UIを表示
        }
    }

    private void ForceSelectRemainingCards()
    {
        Debug.Log("時間切れ！残りのカードをランダムで選択");

        player1Selector.ForceCompleteSelectionIfNeeded();
        player2Selector.ForceCompleteSelectionIfNeeded();

        if (selectionTimerText != null)
        {
            selectionTimerText.gameObject.SetActive(false);
        }
    }

    private void ShowDamagePopup(GameObject targetCard, int damageAmount)
    {
        if (damagePopupPrefab == null || targetCard == null) return;

        // 吹き出しを生成
        GameObject popup = Instantiate(damagePopupPrefab, targetCard.transform.parent);

        // 初期位置を設定
        popup.transform.position = targetCard.transform.position + damagePopupOffset;

        // テキストを更新
        TMP_Text text = popup.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = damageAmount.ToString();
        }

        // アニメーション（上にふわっと動いて消える）
        Sequence seq = DOTween.Sequence();
        seq.Append(popup.transform.DOMoveY(popup.transform.position.y + 5f, 0.8f).SetEase(Ease.OutQuad));
        seq.Join(popup.GetComponent<CanvasGroup>().DOFade(0f, 0.8f));
        seq.OnComplete(() => Destroy(popup));
    }


    void StartRerollPhase()
    {
        rerollTimer = 0f;
        isRerollTimerRunning = true;
        p1WantsReroll = false;
        p2WantsReroll = false;

        selectionTimerText.gameObject.SetActive(true); // UI表示

        if (directionalCardSelector != null)
        {
            directionalCardSelector.canSelect = false; // リロール中は無効化
        }
        player1Selector.canSelect = false;
        player2Selector.canSelect = false;
    }

    void EnterRerollSelectionPhase()
    {
        if (p1WantsReroll)
            player1Selector.RefreshHand();

        if (p2WantsReroll)
            player2Selector.RefreshHand();

        // UI消す or フェーズ切り替えの準備
        selectionTimerText.gameObject.SetActive(false);

        // 選択フェーズへ移行
        StartSelectionPhase();

        if (directionalCardSelector != null)
        {
            directionalCardSelector.canSelect = true; // 選択可能に戻す
        }
        player1Selector.canSelect = true;
        player2Selector.canSelect = true;
    }

    void ToggleComboListUI(int playerId)
    {
        if (playerId == 1)
        {
            if (player1ComboListUIInstance == null)
            {
                player1ComboListUIInstance = Instantiate(comboListUIPrefab, player1ComboListParent, false);
                player1ComboListUIInstance.transform.localPosition = new Vector3(-Screen.width, 0, -10f);

                //RectTransform rt = player1ComboListUIInstance.GetComponent<RectTransform>();
                //if (rt != null)
                //{
                //    rt.anchorMin = Vector2.zero;
                //    rt.anchorMax = Vector2.one;
                //    rt.sizeDelta = Vector2.zero;
                //    rt.anchoredPosition = Vector2.zero;
                //    rt.localScale = Vector3.one;
                //}

                var image = player1ComboListUIInstance.GetComponent<Image>();
                if (image != null && player1Deck != null)
                {
                    image.sprite = player1Deck.comboListImage;
                }

                player1ComboListUIInstance.SetActive(true);
                player1ComboListUIInstance.transform.SetAsLastSibling(); // ここで親の中で一番上に

                player1ComboListUIInstance.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutCubic);
            }
            else
            {
                if (player1ComboListUIInstance.activeSelf)
                {
                    player1ComboListUIInstance.transform.DOLocalMove(new Vector3(-Screen.width, 0, 0), 0.5f)
                        .SetEase(Ease.InCubic)
                        .OnComplete(() => player1ComboListUIInstance.SetActive(false));
                }
                else
                {
                    player1ComboListUIInstance.SetActive(true);
                    player1ComboListUIInstance.transform.SetAsLastSibling(); // 再表示時も前面に
                    player1ComboListUIInstance.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutCubic);
                }
            }
        }
        else if (playerId == 2)
        {
            if (player2ComboListUIInstance == null)
            {
                player2ComboListUIInstance = Instantiate(comboListUIPrefab, player2ComboListParent, false);
                player2ComboListUIInstance.transform.localPosition = new Vector3(-Screen.width, 0, -10f);

                //RectTransform rt = player2ComboListUIInstance.GetComponent<RectTransform>();
                //if (rt != null)
                //{
                //    rt.anchorMin = Vector2.zero;
                //    rt.anchorMax = Vector2.one;
                //    rt.sizeDelta = Vector2.zero;
                //    rt.anchoredPosition = Vector2.zero;
                //    rt.localScale = Vector3.one;
                //}

                var image = player2ComboListUIInstance.GetComponent<Image>();
                if (image != null && player2Deck != null)
                {
                    image.sprite = player2Deck.comboListImage;
                }

                player2ComboListUIInstance.SetActive(true);
                player2ComboListUIInstance.transform.SetAsLastSibling();

                player2ComboListUIInstance.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutCubic);
            }
            else
            {
                if (player2ComboListUIInstance.activeSelf)
                {
                    player2ComboListUIInstance.transform.DOLocalMove(new Vector3(Screen.width, 0, 0), 0.5f)
                        .SetEase(Ease.InCubic)
                        .OnComplete(() => player2ComboListUIInstance.SetActive(false));
                }
                else
                {
                    player2ComboListUIInstance.SetActive(true);
                    player2ComboListUIInstance.transform.SetAsLastSibling(); // 再表示時も最前面に
                    player2ComboListUIInstance.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutCubic);
                }
            }
        }
    }

    private IEnumerator PlayBannerAnimation(Sprite bannerSprite)
    {
        // nullチェックと初期化
        if (bannerImage == null || bannerRectTransform == null)
            yield break;

        bannerImage.sprite = bannerSprite;
        bannerImage.SetNativeSize();
        bannerRectTransform.gameObject.SetActive(true);

        //ラウンド開始SEを再生
        if (sePlayer != null)
        {
            sePlayer.PlayRoundStartSE();
        }

        // 画面外（左）に配置
        RectTransform bannerRect = bannerRectTransform.GetComponent<RectTransform>();
        bannerRect.anchoredPosition = new Vector2(-Screen.width, 0);

        // 左から中央にスライドイン（0.5秒）
        yield return bannerRect.DOAnchorPos(Vector2.zero, 0.5f)
            .SetEase(Ease.OutCubic)
            .WaitForCompletion();

        // 少し表示（1秒くらい）
        yield return new WaitForSeconds(1.0f);

        // 右にスライドアウト（0.5秒）
        yield return bannerRect.DOAnchorPos(new Vector2(Screen.width, 0), 0.5f)
            .SetEase(Ease.InCubic)
            .WaitForCompletion();

        bannerRectTransform.gameObject.SetActive(false);
    }

    IEnumerator StartBattleSequence()
    {
        yield return new WaitForSeconds(1f); // シーン入り後の余白

        // バトル開始帯
        yield return ShowBanner(battleStartSprite);

        // ラウンド1開始帯
        yield return ShowBanner(round1Sprite);

        // 通常処理開始（選択フェーズ開始）
        StartRerollPhase();
    }

    private IEnumerator ShowBanner(Sprite bannerSprite)
    {
        bannerImage.sprite = bannerSprite;
        bannerImage.SetNativeSize(); // 画像に応じてサイズを合わせたい場合

        bannerRectTransform.anchoredPosition = new Vector2(-Screen.width, 0);
        bannerRectTransform.gameObject.SetActive(true);

        // スライドイン
        yield return bannerRectTransform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();

        yield return new WaitForSeconds(1f); // 表示時間

        // スライドアウト
        yield return bannerRectTransform.DOAnchorPos(new Vector2(Screen.width, 0), 0.5f).SetEase(Ease.InCubic).WaitForCompletion();

        bannerRectTransform.gameObject.SetActive(false);
    }

}
