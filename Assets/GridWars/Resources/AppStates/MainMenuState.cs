using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class MainMenuState : AppState {
	UIButton internetPvpButton;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		CameraController cc = Object.FindObjectOfType<CameraController>();
		cc.MainMenuEntered();

		if (QualitySettings.vSyncCount > 0) {
			QualitySettings.vSyncCount = 2;
		}
		else {
			Application.targetFrameRate = 30;
		}

		app.battlefield.SoftReset();
		network.Reset();

		battlefield.isInternetPVP = false;
		battlefield.isAiVsAi = false;
		battlefield.player1.npcModeOn = false;
		battlefield.player2.npcModeOn = false;

		ShowMainMenu();

		matchmaker.matchmakerState.MainMenuEntered();
		matchmaker.menu.Show();

		App.shared.SoundtrackNamed("MenuBackgroundMusic").Play();
	}

	public override void WillExit() {
		base.WillExit();

		if (QualitySettings.vSyncCount > 0) {
			QualitySettings.vSyncCount = 1;
		}
		else {
			Application.targetFrameRate = 60;
		}

		matchmaker.matchmakerState.MainMenuExited();
		matchmaker.menu.Close();
		matchmaker.menu.Hide();
	}
		
	private static bool _needsInitialFadeIn = true;

	void ShowMainMenu() {
		app.ResetMenu();
		internetPvpButton = menu.AddNewButton().SetText("Internet PVP").SetAction(InternetPvpClicked);
		menu.AddItem(UI.MenuItem("Shared Screen PVP", SharedScreenPvpClicked));
		menu.AddItem(UI.MenuItem("Player vs AI", PlayerVsCompClicked));
		menu.AddItem(UI.MenuItem("AI vs AI", CompVsCompClicked));
		menu.AddItem(UI.MenuItem("Tutorial", Tutorial));
		menu.AddItem(UI.MenuItem("Chat", ChatClicked));
        menu.AddItem(UI.MenuItem("Options", OptionsClicked));
		menu.AddItem(UI.MenuItem("Quit", Quit));
		menu.Show();

		if (_needsInitialFadeIn) {
			menu.backgroundColor = Color.black;
			menu.targetBackgroundColor = Color.clear;
			_needsInitialFadeIn = false;

			//PlayerPrefs.DeleteAll();
			if (ShouldPlayTutorial()) {
				Tutorial();
			}
		}
	}

	void InternetPvpClicked() {
		Analytics.CustomEvent("InternetPvpClicked", new Dictionary<string, object> {
			{ "playTime", Time.timeSinceLevelLoad }
		});

		if (matchmaker.isConnected) {
			matchmaker.matchmakerState.MainMenuInternetPvpClicked();
		}
		else {
			menu.Reset();
			menu.AddNewText()
				.SetText("Unable to connect to the server.\n\nInternet matches disabled.");
			menu.AddNewButton()
				.SetText("Close")
				.SetAction(ShowMainMenu)
				.SetIsBackItem(true);
			menu.Show();
		}
	}

	public void MatchmakerConnected() {
		if (internetPvpButton != null) {
			internetPvpButton.UseDefaultStyle();
			if (menu.hasFocus) {
				menu.soundsEnabled = false;
				menu.Focus(); //reselect to update style
				menu.soundsEnabled = true;
			}
		}
	}

	public void MatchmakerDisconnected() {
		if (internetPvpButton != null) {
			internetPvpButton.UseAlertStyle();
			if (menu.hasFocus) {
				menu.soundsEnabled = false;
				menu.Focus(); //reselect to update style
				menu.soundsEnabled = true;
			}
		}
	}

	void SharedScreenPvpClicked() {
		battlefield.player1.isLocal = true;
		battlefield.player2.isLocal = true;

		battlefield.player2.npcModeOn = false;
		battlefield.player2.npcModeOn = false;

		battlefield.player1.isTutorialMode = false;
		battlefield.player2.isTutorialMode = false;

        Analytics.CustomEvent("SharedScreenPvPClicked", new Dictionary<string, object>
                {
                    { "playTime", Time.timeSinceLevelLoad }
                });
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

        Analytics.CustomEvent("PlayerVsCompClicked", new Dictionary<string, object>
                {
                    { "playTime", Time.timeSinceLevelLoad }
                });

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

        Analytics.CustomEvent("CompVsCompClicked", new Dictionary<string, object>
                {
                    { "playTime", Time.timeSinceLevelLoad }
                });

		TransitionTo(new WaitForBoltState());
	}

	void ChatClicked() {
        Analytics.CustomEvent("ChatClicked", new Dictionary<string, object>
                {
                    { "playTime", Time.timeSinceLevelLoad }
                });

		Application.OpenURL("http://slack.baremetalgame.com/");
	}

    void OptionsClicked(){
        TransitionTo(new OptionsMenuState());
    }
        
	bool ShouldPlayTutorial() {
		return PlayerPrefs.GetInt("HasPlayedTutorial", 0) == 0;
	}

	void Tutorial() {

		battlefield.isAiVsAi = true;

		battlefield.player1.isLocal = false;
		battlefield.player1.npcModeOn = true;
		battlefield.player1.isTutorialMode = true;

		battlefield.player2.isLocal = false;
		battlefield.player2.npcModeOn = true;
		battlefield.player2.isTutorialMode = true;

        Analytics.CustomEvent("TutorialClicked", new Dictionary<string, object>
                {
                    { "playTime", Time.timeSinceLevelLoad }
                });

		TransitionTo(new WaitForBoltState());
	}
		
	void Quit() {
		Application.Quit();

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
}
