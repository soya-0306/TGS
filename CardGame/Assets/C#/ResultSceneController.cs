using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultSceneController : MonoBehaviour
{
    public TMP_Text resultText;
    public AudioClip resultSound;          // 再生したい音をインスペクターで設定
    private AudioSource audioSource;       // 再生用AudioSource

    void Start()
    {
        if (resultText != null && GameResultManager.Instance != null)
        {
            resultText.text = GameResultManager.Instance.finalResultText;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (resultSound != null)
        {
            audioSource.PlayOneShot(resultSound);
            Debug.Log("🎵 リザルト音を再生しました。");
        }
        else
        {
            Debug.Log("⚠️ リザルト音が未設定のため、音は再生されませんでした。");
        }
    }

    void Update()
    {
        // Xbox Bボタン（JoystickButton1）または キーボードのBキー で戻る
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.B))
        {
            ReturnToTitle();
        }
    }

    public void ReturnToTitle()
    {
        //SceneManager.LoadScene("TitleScene");
        FadeManager.Instance.LoadScene("TitleScene", 2.0f);
    }
}
