using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class MainMenuState : AppState {
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

		app.ResetMenu();
		menu.AddItem(UI.MenuItem("Play", PlayClicked));
		menu.AddItem(UI.MenuItem("PVP Ladder", LadderClicked));
		menu.AddItem(UI.MenuItem("Account", AccountClicked));
		menu.AddItem(UI.MenuItem("Community", ChatClicked));
		menu.AddItem(UI.MenuItem("Options", OptionsClicked));
		menu.AddItem(UI.MenuItem("Quit", Quit));
		menu.Show();

		if (_needsInitialFadeIn) {
			menu.backgroundColor = Color.black;
			menu.targetBackgroundColor = Color.clear;
			_needsInitialFadeIn = false;
		}
			
		matchmaker.menu.Show();
		ConnectMatchmakerMenu();

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
	}
		
	private static bool _needsInitialFadeIn = true;

	void PlayClicked() {
		/*Analytics.CustomEvent("PlayClicked", new Dictionary<string, object>{
			{ "playTime", Time.timeSinceLevelLoad }
		});*/

		TransitionTo(new PlayMenuState());
	}

	void LadderClicked() {
		TransitionTo(new LadderState());
	}

	void AccountClicked() {
		/*Analytics.CustomEvent("AccountClicked", new Dictionary<string, object>{
			{ "playTime", Time.timeSinceLevelLoad }
		});*/

		TransitionTo(new AccountMenuState());
	}

	void ChatClicked() {
       /* Analytics.CustomEvent("ChatClicked", new Dictionary<string, object>
                {
                    { "playTime", Time.timeSinceLevelLoad }
                });*/

		Application.OpenURL("http://slack.baremetalgame.com/");
	}

    void OptionsClicked(){
        TransitionTo(new OptionsMenuState());
    }
		
	void Quit() {
		Application.Quit();

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
}
