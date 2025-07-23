using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro�p

public class PlayerHP : MonoBehaviour
{
    public int playerId = 1; // 1 or 2
    public Image hpFillImage;

    public TMP_Text hpText;  // �ǉ��FHP�e�L�X�gUI

    private int maxHP = 10;
    public int currentHP;

    void Start()
    {
        currentHP = maxHP;
        UpdateHPBar();
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        UpdateHPBar();

        Debug.Log($"Player{playerId} took {amount} damage. Current HP: {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log($"Player{playerId} is dead!");
            // �����ŃQ�[���I�[�o�[���������Ă�OK
        }
    }

    private void UpdateHPBar()
    {
        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = (float)currentHP / maxHP;
        }

        if (hpText != null)
        {
            hpText.text = $"{currentHP}/{maxHP}";
        }
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        UpdateHPBar();
        Debug.Log($"Player{playerId} healed {amount} HP. Current HP: {currentHP}");
    }

}
