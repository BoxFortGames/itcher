using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamesList : MonoBehaviour {

    public GameListEntry gameListEntryPrefab;

    protected ToggleGroup toggleGroup;
    protected ItcherGame activeGame;
    protected List<GameListEntry> gameListEntries = new List<GameListEntry>();

    public ItcherGame Game {  get { return activeGame; } }
    
	void Start () {
        toggleGroup = GetComponent<ToggleGroup>();
	}

    public void SetGames(ItcherGame [] games)
    {
        activeGame = null;
        foreach(GameListEntry gle in gameListEntries)
        {
            Destroy(gle.gameObject);
        }
        gameListEntries.Clear();

        foreach (ItcherGame game in games)
        {
            GameListEntry gle = Instantiate<GameListEntry>(gameListEntryPrefab);
            gle.gamesList = this;
            gle.SetToggleGroup(toggleGroup);
            gle.Game = game;
            gle.transform.SetParent(transform);
            gle.transform.localScale = Vector3.one;

            if (activeGame == null)
            {
                activeGame = game;
                gle.GetComponent<Toggle>().isOn = true;
            }

            gameListEntries.Add(gle);
        }
    }

    public void SetActiveGame(ItcherGame game)
    {
        activeGame = game;
    }
}
