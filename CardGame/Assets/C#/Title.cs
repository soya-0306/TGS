using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyKeyToSelectScene : MonoBehaviour
{
    private bool hasLoaded = false;

    void Update()
    {
        if (hasLoaded) return;

        // ��������̃L�[��{�^���������ꂽ��
        if (Input.anyKeyDown)
        {
            hasLoaded = true; // ��d�ǂݍ��ݖh�~���킷
            SceneManager.LoadScene("CharacterSelectScene");
        }
    }
}
