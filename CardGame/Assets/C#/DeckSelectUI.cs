using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DeckSelectUI : MonoBehaviour
{
    public AudioClip buttonClickSound;  // インスペクターで設定する
    private AudioSource audioSource;

    [SerializeField] private GameObject blockerPanel;

    private bool isOperationBlocked = false;

    // ここにTwoPlayerUISelectorの参照
    public TwoPlayerUISelector uiSelector;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        if (blockerPanel != null)
        {
            blockerPanel.SetActive(false);
        }
    }

    private IEnumerator PlaySEAndLoadScene()
    {
        // 操作禁止開始
        isOperationBlocked = true;
        if (blockerPanel != null)
        {
            blockerPanel.SetActive(true);
        }

        // TwoPlayerUISelectorの操作禁止もセット
        if (uiSelector != null)
        {
            uiSelector.isOperationBlocked = true;
        }

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
        if (isOperationBlocked) return;
        StartCoroutine(PlaySEAndLoadScene());
    }
}
