using UnityEngine;

public static class ComboEffectManager
{
    public static void ApplyEffect(int effectID, PlayerHP player)
    {
        switch (effectID)
        {
            case 0:
                // �������Ȃ�
                break;
            case 1:
                HealOne(player);
                break;
            case 2:
                // �����_�������ւ��̎w�����o��
                Debug.Log($"Player{player.playerId} �̓����_�������ւ����ʂ������I");
                break;
            default:
                Debug.LogWarning($"����`�̃R���{����ID: {effectID}");
                break;
        }
    }

    private static void HealOne(PlayerHP player)
    {
        player.Heal(1);
        Debug.Log($"Player{player.playerId} �̓R���{���ʂ�1�񕜁I");
    }

    public static bool IsRandomReplaceNeeded(int effectID)
    {
        return effectID == 2;  // ��FID 2���������_���u��������K�v�Ƃ���
    }
}
