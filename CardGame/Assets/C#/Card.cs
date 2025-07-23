using UnityEngine;

/// <summary>
/// �J�[�h�̎�ނ��`�����񋓌^
/// </summary>
public enum CardType
{
    Rock,       // �O�[
    Scissors,   // �`���L
    Paper,      // �p�[
    Shield      // �V�[���h�i�h��j
}

/// <summary>
/// �J�[�h1�����̃f�[�^�N���X
/// </summary>
[System.Serializable]
public class Card
{
    public CardType Type;     // �J�[�h�̎��
    public string Name;       // �\���p�̖��O
    public Sprite Icon;       // UI�p�̃A�C�R���i�摜�j

    /// <summary>
    /// �R���X�g���N�^�i�蓮�����p�j
    /// </summary>
    public Card(CardType type, string name, Sprite icon = null)
    {
        Type = type;
        Name = name;
        Icon = icon;
    }

    /// <summary>
    /// ���̃J�[�h���U�����ǂ����𔻒�i�V�[���h�͍U������Ȃ��j
    /// </summary>
    public bool IsAttackCard()
    {
        return Type != CardType.Shield;
    }

    /// <summary>
    /// ���̃J�[�h���h��i�V�[���h�j���ǂ���
    /// </summary>
    public bool IsShield()
    {
        return Type == CardType.Shield;
    }

    /// <summary>
    /// ����񂯂�̏��s����p�F���̃J�[�h������ɏ��Ă邩
    /// </summary>
    public bool Beats(Card other)
    {
        if (this.Type == CardType.Rock && other.Type == CardType.Scissors) return true;
        if (this.Type == CardType.Scissors && other.Type == CardType.Paper) return true;
        if (this.Type == CardType.Paper && other.Type == CardType.Rock) return true;
        return false;
    }
}
