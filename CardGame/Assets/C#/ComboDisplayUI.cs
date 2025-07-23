using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class ComboDisplayUI : MonoBehaviour
{
    [SerializeField] private GameObject comboPanelPrefab;  // �R���{�\���p�l���̃v���n�u
    [SerializeField] private Transform comboPanelParent;   // �p�l���̐eTransform
    [SerializeField] private GameObject cardIconPrefab;    // �J�[�h�A�C�R���iImage�j�v���n�u

    private List<GameObject> activeComboPanels = new List<GameObject>();

    /// <summary>
    /// �R���{�̃��X�g���󂯎����UI�\�����X�V����
    /// </summary>
    public void ShowCombos(List<Combo> combosToShow, Dictionary<CardType, Sprite> cardIconTable)
    {
        ClearComboPanels();

        foreach (var combo in combosToShow)
        {
            GameObject panel = Instantiate(comboPanelPrefab, comboPanelParent);
            activeComboPanels.Add(panel);

            // �p�l�����ɃR���{���e�L�X�g�ƃJ�[�h�A�C�R�����Z�b�g����z��
            Text comboNameText = panel.transform.Find("ComboNameText").GetComponent<Text>();
            comboNameText.text = combo.sequence.Count > 0 ? $"Combo: {combo.sequence.Count} cards" : "Combo";

            // �J�[�h�A�C�R�������ɕ\��
            Transform cardsParent = panel.transform.Find("Cards");
            foreach (var cardType in combo.sequence)
            {
                GameObject iconGO = Instantiate(cardIconPrefab, cardsParent);
                Image iconImage = iconGO.GetComponent<Image>();

                if (cardIconTable.TryGetValue(cardType, out var iconSprite))
                {
                    iconImage.sprite = iconSprite;
                }
                else
                {
                    Debug.LogWarning($"CardType {cardType} �̃A�C�R����������܂���B");
                }
            }
        }
    }

    /// <summary>
    /// �����̃R���{�p�l����S������
    /// </summary>
    public void ClearComboPanels()
    {
        foreach (var panel in activeComboPanels)
        {
            Destroy(panel);
        }
        activeComboPanels.Clear();
    }
}
