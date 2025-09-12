using UnityEngine;
using UnityEngine.UI;

public static class GameResetManager
{
    public static void ResetAll()
    {
        Debug.Log("[GameResetManager] ResetAll: clearing persistent state...");

        // GameResult のみクリア
        if (GameResultManager.Instance != null)
            GameResultManager.Instance.finalResultText = "";

        // DeckSelector のみクリア
        if (DeckSelector.Instance != null)
        {
            DeckSelector.Instance.player1Deck = null;
            DeckSelector.Instance.player2Deck = null;
            DeckSelector.Instance.currentSelecting = PlayerSelecting.Player1;
        }

        // ComboEffectManager のみリセット
        ComboEffectManager.ResetEffects();

        // ※ PlayerHP や UI のリセットはやらない！
        // シーンロードで復活するのでここで触る必要なし

        Debug.Log("[GameResetManager] ResetAll complete.");
    }

}
