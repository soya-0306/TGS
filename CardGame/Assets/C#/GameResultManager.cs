using UnityEngine;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance;

    public string finalResultText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // シーンまたいでも残す
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
