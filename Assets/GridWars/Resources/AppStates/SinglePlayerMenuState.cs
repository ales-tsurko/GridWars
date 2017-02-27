using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class SinglePlayerMenuState : AppState {
	UIButton internetPvpButton;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		ShowMenu();
	}

	void ShowMenu() {
		menu.Reset();
        menu.AddItem(UI.MenuItem("Campaign", PvELadderClicked));  //commented while still adding
		menu.AddItem(UI.MenuItem("Player vs AI", PlayerVsCompClicked));
		menu.AddItem(UI.MenuItem("AI vs AI", CompVsCompClicked));
		menu.AddItem(UI.MenuItem("Back", Back).SetIsBackItem(true));
		menu.Show();
	}

	void SharedScreenPvpClicked() {
		battlefield.player1.isLocal = true;
		battlefield.player2.isLocal = true;

		battlefield.player2.npcModeOn = false;
		battlefield.player2.npcModeOn = false;

		battlefield.player1.isTutorialMode = false;
		battlefield.player2.isTutorialMode = false;

		/*Analytics.CustomEvent("SharedScreenPvPClicked", new Dictionary<string, object>
			{
				{ "playTime", Time.timeSinceLevelLoad }
			});*/
		TransitionTo(new BindInputsToPlayersState());
	}

	void PlayerVsCompClicked() {

		if (UnityEngine.Random.value < 0.5f) {
			battlefield.player1.isLocal = true;
			battlefield.player2.isLocal = false;

			battlefield.player1.npcModeOn = false;
			battlefield.player2.npcModeOn = true;
		} else {
			battlefield.player1.isLocal = false;
			battlefield.player2.isLocal = true;

			battlefield.player1.npcModeOn = true;
			battlefield.player2.npcModeOn = false;
		}

		battlefield.player1.isTutorialMode = false;
		battlefield.player2.isTutorialMode = false;

		/*Analytics.CustomEvent("PlayerVsCompClicked", new Dictionary<string, object>
			{
				{ "playTime", Time.timeSinceLevelLoad }
			});*/

		TransitionTo(new WaitForBoltState());
	}

    void PvELadderClicked (){ 
        battlefield.player1.isLocal = true;
        battlefield.player2.isLocal = false;

        battlefield.player1.npcModeOn = false;
        battlefield.player2.npcModeOn = true;

        battlefield.player1.isTutorialMode = false;
        battlefield.player2.isTutorialMode = false;

        battlefield.isPvELadder = true;
        battlefield.pveLadderLevel = 1;

        TransitionTo(new WaitForBoltState());
    }

	void CompVsCompClicked() {
		battlefield.isAiVsAi = true;
		battlefield.player1.isLocal = false;
		battlefield.player2.isLocal = false;

		battlefield.player1.npcModeOn = true;
		battlefield.player2.npcModeOn = true;

		battlefield.player1.isTutorialMode = false;
		battlefield.player2.isTutorialMode = false;

		/*Analytics.CustomEvent("CompVsCompClicked", new Dictionary<string, object>
			{
				{ "playTime", Time.timeSinceLevelLoad }
			});*/

		TransitionTo(new WaitForBoltState());
	}

	void ViewLeaderboard() {
		TransitionTo(new SingleplayerLeaderboardMenuState());
	}

	void Back() {
		TransitionTo(new PlayMenuState());
	}
}
