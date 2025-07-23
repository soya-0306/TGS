using UnityEngine;

/// <summary>
/// カードの種類を定義した列挙型
/// </summary>
public enum CardType
{
    Rock,       // グー
    Scissors,   // チョキ
    Paper,      // パー
    Shield      // シールド（防御）
}

/// <summary>
/// カード1枚分のデータクラス
/// </summary>
[System.Serializable]
public class Card
{
    public CardType Type;     // カードの種類
    public string Name;       // 表示用の名前
    public Sprite Icon;       // UI用のアイコン（画像）

    /// <summary>
    /// コンストラクタ（手動生成用）
    /// </summary>
    public Card(CardType type, string name, Sprite icon = null)
    {
        Type = type;
        Name = name;
        Icon = icon;
    }

    /// <summary>
    /// このカードが攻撃かどうかを判定（シールドは攻撃じゃない）
    /// </summary>
    public bool IsAttackCard()
    {
        return Type != CardType.Shield;
    }

    /// <summary>
    /// このカードが防御（シールド）かどうか
    /// </summary>
    public bool IsShield()
    {
        return Type == CardType.Shield;
    }

    /// <summary>
    /// じゃんけんの勝敗判定用：このカードが相手に勝てるか
    /// </summary>
    public bool Beats(Card other)
    {
        if (this.Type == CardType.Rock && other.Type == CardType.Scissors) return true;
        if (this.Type == CardType.Scissors && other.Type == CardType.Paper) return true;
        if (this.Type == CardType.Paper && other.Type == CardType.Rock) return true;
        return false;
    }
}
