using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckSelectUI : MonoBehaviour
{
    //public DeckDefinition deck1;
    //public DeckDefinition deck2;

    //private Button button;

    //void Start()
    //{
    //    button = GetComponent<Button>();
    //    button.onClick.AddListener(OnClick);
    //}

    //public void SelectDeckForPlayer1(int deckIndex)
    //{
    //    DeckSelector.Instance.player1Deck = (deckIndex == 0) ? deck1 : deck2;
    //}

    //public void SelectDeckForPlayer2(int deckIndex)
    //{
    //    DeckSelector.Instance.player2Deck = (deckIndex == 0) ? deck1 : deck2;
    //}

    //public void StartBattle()
    //{
    //    SceneManager.LoadScene("BattleScene");
    //}

    public void OnClick()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
