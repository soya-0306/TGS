using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultSceneController : MonoBehaviour
{
    public TMP_Text resultText;

    void Start()
    {
        if (resultText != null && GameResultManager.Instance != null)
        {
            resultText.text = GameResultManager.Instance.finalResultText;
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
        SceneManager.LoadScene("TitleScene");
    }
}
