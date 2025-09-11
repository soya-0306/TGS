using UnityEngine;

public static class ComboEffectManager
{

    // 次のカードにダメージ補正が入るかどうかのフラグ
    private static bool nextDamageUpActive = false;
    private static int nextDamageBonus = 0;

    public static void ApplyEffect(int effectID, PlayerHP player, int effectValue = 0)
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
            case 3:
                // あいこの次ダメージUP
                ActivateNextDamageUp(effectValue);
                Debug.Log($"Player{player.playerId} は次のカードに +{effectValue} ダメージが付与される！");
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

    // あいこの次に出すカードの威力UPを有効化
    private static void ActivateNextDamageUp(int bonus)
    {
        nextDamageUpActive = true;
        nextDamageBonus = bonus;
    }

    // BattleManager 側で呼んでダメージを補正（呼ぶとフラグはリセットされる）
    public static int GetModifiedDamage(int baseDamage)
    {
        if (nextDamageUpActive)
        {
            int result = baseDamage + nextDamageBonus;
            nextDamageUpActive = false; // 一度きり
            nextDamageBonus = 0;
            return result;
        }
        return baseDamage;
    }

    public static bool IsRandomReplaceNeeded(int effectID)
    {
        return effectID == 2;  // 例：ID 2だけランダム置き換えを必要とする
    }

    public static void ResetEffects()
    {
        nextDamageBonus = 0;
    }
}
