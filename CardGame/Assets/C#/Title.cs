using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyKeyToSelectScene : MonoBehaviour
{
    private bool hasLoaded = false;

    void Update()
    {
        if (hasLoaded) return;

        // 何かしらのキーやボタンが押されたら
        if (Input.anyKeyDown)
        {
            hasLoaded = true; // 二重読み込み防止ごわす
            SceneManager.LoadScene("CharacterSelectScene");
        }
    }
}
