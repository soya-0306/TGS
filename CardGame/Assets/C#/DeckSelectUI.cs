using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DeckSelectUI : MonoBehaviour
{
    public AudioClip buttonClickSound;  // インスペクターで設定する
    private AudioSource audioSource;

    //public DeckDefinition deck1;
    //public DeckDefinition deck2;

    //private Button button;

    //void Start()
    //{
    //    button = GetComponent<Button>();
    //    button.onClick.AddListener(OnClick);
    //}

    //public void SelectDeckForPlayer1(int deckIndex)
    //{
    //    DeckSelector.Instance.player1Deck = (deckIndex == 0) ? deck1 : deck2;
    //}

    //public void SelectDeckForPlayer2(int deckIndex)
    //{
    //    DeckSelector.Instance.player2Deck = (deckIndex == 0) ? deck1 : deck2;
    //}

    //public void StartBattle()
    //{
    //    SceneManager.LoadScene("BattleScene");
    //}
    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private IEnumerator PlaySEAndLoadScene()
    {
        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
            Debug.Log("🔊 ボタンクリック音再生");

            // SEが終わるまで待つ
            yield return new WaitForSeconds(buttonClickSound.length);
        }
        else
        {
            Debug.LogWarning("⚠️ buttonClickSoundが未設定です。SEなしでシーン遷移します。");
        }

        //SceneManager.LoadScene("BattleScene");
        FadeManager.Instance.LoadScene("BattleScene", 2.0f);
    }

    public void OnClick()
    {
        StartCoroutine(PlaySEAndLoadScene());
    }
}
