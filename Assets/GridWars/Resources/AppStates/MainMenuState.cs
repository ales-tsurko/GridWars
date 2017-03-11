using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class MainMenuState : AppState {
	UIMenu leaderboardMenu;
	JSONObject pvpLadderResult;

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
		battlefield.isPvELadder = false;

		battlefield.player1.npcModeOn = false;
		battlefield.player2.npcModeOn = false;

		battlefield.player1.inputs = null;
		battlefield.player2.inputs = null;

		app.ResetMenu();
		menu.AddItem(UI.MenuItem("Play", PlayClicked));
		menu.AddItem(UI.MenuItem("Leaderboards", Leaderboards));
		menu.AddItem(UI.MenuItem("Account", AccountClicked));
		menu.AddItem(UI.MenuItem("Community", ChatClicked));
		menu.AddItem(UI.MenuItem("Options", OptionsClicked));
		menu.AddItem(UI.MenuItem("Quit", Quit));
		menu.Show();

		if (matchmaker.isConnected) {
			FetchLeaderboards(null);
		}
		else {
			app.notificationCenter.NewObservation()
				.SetNotificationName(Matchmaker.MatchmakerConnectedNotification)
				.SetAction(FetchLeaderboards)
				.Add();
		}

		if (_needsInitialFadeIn) {
			menu.backgroundColor = Color.black;
			menu.targetBackgroundColor = Color.clear;
			_needsInitialFadeIn = false;
		}
			
		matchmaker.menu.Show();
		ConnectMatchmakerMenu();

		App.shared.SoundtrackNamed("MenuBackgroundMusic").Play();
	}

	void FetchLeaderboards(Notification n) {
		app.notificationCenter.RemoveObserver(this);

		app.notificationCenter.NewObservation()
			.SetNotificationName(matchmaker.ReceivedMessageNotificationName("requestLadder"))
			.SetAction(MatchmakerReceivedRequestLadder)
			.SetSender(matchmaker)
			.Add();

		matchmaker.Send("requestLadder");
	}

	void MatchmakerReceivedRequestLadder(Notification n) {
		pvpLadderResult = n.data as JSONObject;

		app.notificationCenter.NewObservation()
			.SetNotificationName(matchmaker.ReceivedMessageNotificationName("requestLeaderboard"))
			.SetAction(MatchmakerReceivedRequestLeaderboard)
			.SetSender(matchmaker)
			.Add();

		JSONObject data = new JSONObject();

		data.AddField("type", "Level");
		data.AddField("start", "1451606400000");
		data.AddField("end", "4607280000000");
		matchmaker.Send("requestLeaderboard", data);
	}

	void MatchmakerReceivedRequestLeaderboard(Notification n) {
		leaderboardMenu = UI.Menu();
		leaderboardMenu.anchor = MenuAnchor.TopCenter;
		leaderboardMenu.vMargin = 120f;

		var item = leaderboardMenu.AddNewText();
		item.innerMargins = new Vector2(0f, 0.5f);
		item.SetText("top players:");
		item.rainbowCyclePeriod = 2f;
		//item.UseRainbowStyle();
		item.doesType = true;

		var pvpLeaders = pvpLadderResult.GetField("ladder").list;

		if (pvpLeaders.Count > 0) {
			item = leaderboardMenu.AddNewText();
			item.innerMargins = new Vector2(0f, 0.5f);
			item.SetText(pvpLeaders[0].GetField("screenName").str + " (PVP)");
			item.UseRainbowStyle();
			item.doesType = true;
		}

		var pveLeaders = (n.data as JSONObject).list;
		if (pveLeaders.Count > 0) {
			item = leaderboardMenu.AddNewText();
			item.innerMargins = new Vector2(0f, 0.5f);
			item.SetText(pveLeaders[0].GetField("screenName").str + " (PVE)");
			item.UseRainbowStyle();
			item.doesType = true;
		}

		leaderboardMenu.Show();
	}

	public override void WillExit() {
		base.WillExit();

		app.notificationCenter.RemoveObserver(this);

		if (leaderboardMenu != null) {
			leaderboardMenu.Destroy();
		}

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

	void Leaderboards() {
		TransitionTo(new LeaderboardsMenuState());
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

		Application.OpenURL("https://discordapp.com/invite/qAjsB8K");
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
