using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int playerId = 1; // 1 or 2

    [Header("宝石オブジェクト（10個）")]
    public GameObject[] gemObjects = new GameObject[10];

    [Header("マテリアル設定")]
    public Material normalMaterial;   // 明るい宝石
    public Material damagedMaterial;  // 暗い宝石

    private int maxHP = 1;
    public int currentHP;

    void Start()
    {
        currentHP = maxHP;
        UpdateGemMaterials();
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        UpdateGemMaterials();

        Debug.Log($"Player{playerId} took {amount} damage. Current HP: {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log($"Player{playerId} is dead!");
            // ゲームオーバー処理
        }
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        UpdateGemMaterials();

        Debug.Log($"Player{playerId} healed {amount} HP. Current HP: {currentHP}");
    }

    private void UpdateGemMaterials()
    {
        for (int i = 0; i < gemObjects.Length; i++)
        {
            if (gemObjects[i] != null)
            {
                var renderer = gemObjects[i].GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = i < currentHP ? normalMaterial : damagedMaterial;
                }
            }
        }
    }

    // タイトルに戻ったときなどにHPを初期化する
    public void ResetHP()
    {
        currentHP = maxHP;   // maxHP は既にこのクラス内で 10 に設定されています
        UpdateGemMaterials();
    }
}
