using UnityEngine;

public static class ComboEffectManager
{
    public static void ApplyEffect(int effectID, PlayerHP player)
    {
        switch (effectID)
        {
            case 0:
                // 何もしない
                break;
            case 1:
                HealOne(player);
                break;
            case 2:
                // ランダム差し替えの指示を出す
                Debug.Log($"Player{player.playerId} はランダム差し替え効果が発動！");
                break;
            default:
                Debug.LogWarning($"未定義のコンボ効果ID: {effectID}");
                break;
        }
    }

    private static void HealOne(PlayerHP player)
    {
        player.Heal(1);
        Debug.Log($"Player{player.playerId} はコンボ効果で1回復！");
    }

    public static bool IsRandomReplaceNeeded(int effectID)
    {
        return effectID == 2;  // 例：ID 2だけランダム置き換えを必要とする
    }
}
