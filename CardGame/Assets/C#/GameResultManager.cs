using UnityEngine;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance { get; private set; }
    public string finalResultText; // リザルト画面に渡す用

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ これで残す
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
