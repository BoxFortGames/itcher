
using UnityEngine;
using UnityEngine.UI;

public class GameListEntry : MonoBehaviour {

    public Text gameNameText;
    public Toggle toggle;
    public GamesList gamesList;
    
    private ItcherGame game;
    public ItcherGame Game
    {
        get
        {
            return game;
        }

        set
        {
            game = value;
            gameNameText.text = game.title;
        }
    }

    public void SetToggleGroup(ToggleGroup tg)
    {
        toggle.group = tg;
    }

    public void OnSelectionChanged(bool active)
    {
        if(active && gamesList)
        {
            gamesList.SetActiveGame(Game);
        }
    }

}
