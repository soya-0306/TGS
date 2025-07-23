using UnityEngine;
using UnityEngine.UI;

public class DeckSelectButton : MonoBehaviour
{
    public DeckDefinition deck;  // �C���X�y�N�^�[�Ńf�b�L���w��

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        DeckSelector.Instance.SelectDeck(deck);
        Debug.Log($"�f�b�L�u{deck.deckName}�v��I�����܂���");

        // �v���C���[�؂�ւ��i�C�ӁB1P��2P�ցj
        DeckSelector.Instance.TogglePlayer();
    }
}
