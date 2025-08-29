using UnityEngine;

public class KeyInputController : MonoBehaviour
{
    private KeyAnimationController animController;

    void Start()
    {
        // 同じオブジェクトにある KeyAnimationController を参照
        animController = GetComponent<KeyAnimationController>();
        if (animController == null)
        {
            Debug.LogError("KeyAnimationController が見つかりません！");
        }
    }

    void Update()
    {
        if (animController == null) return;

        // 1キー → グー（Fight）
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animController.PlayCardAnimation(CardType.Rock);
        }
        // 2キー → パー（Weapon）
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animController.PlayCardAnimation(CardType.Paper);
        }
        // 3キー → チョキ（Magic）
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animController.PlayCardAnimation(CardType.Scissors);
        }
        // Dキー → ダメージ
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            animController.PlayDamageAnimation();
        }
    }
}
