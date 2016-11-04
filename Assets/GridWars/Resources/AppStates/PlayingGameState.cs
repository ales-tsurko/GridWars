using UnityEngine;
using System.Collections.Generic;

public class PlayingGameState : NetworkDelegateState {
	List<InGameMenu> inGameMenus;

	//AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		Object.FindObjectOfType<CameraController>().EndOrbit();

		battlefield.canCheckGameOver = false;

		network.networkDelegate = this;
		/*
		matchmaker.matchmakerDelegate = null;
		if (matchmaker.isConnected) {
			matchmaker.Disconnect();
		}
		*/

		//do this before ShowInGameMenu
		if (battlefield.isInternetPVP) {
			battlefield.player1.isLocal = BoltNetwork.isServer;
			battlefield.player2.isLocal = BoltNetwork.isClient;
		}

		ShowInGameMenus();

		Battlefield.current.StartGame();

		App.shared.SoundtrackNamed("MenuBackgroundMusic").FadeOut();

		App.shared.PlayAppSoundNamed("GameStart");

		menu.Hide();
	}

	public override void WillExit() {
		base.WillExit();

		menu.Show();

		HideInGameMenus();
	}

	public override void Update() {
		base.Update();

		if (BoltNetwork.isServer && battlefield.canCheckGameOver && battlefield.livingPlayers.Count == 1) {
			var victor = battlefield.livingPlayers[0];
			if (matchmaker.state is MatchmakerPlayingGameState) {
				JSONObject data = new JSONObject();
				data.AddField("isWinner", victor == battlefield.localPlayer1);
				matchmaker.Send("endGame", data);
			}
			EndGame(victor);
		}
		else {
			foreach (var inGameMenu in inGameMenus) {
				inGameMenu.ReadInput();
			}
		}
	}

	public void EndGame(Player victor) {
		var state = new PostGameState();
		state.victoriousPlayer = victor;
		TransitionTo(state);
	}

	//NetworkDelegate

	public override void ZeusDisconnected() {
		base.ZeusDisconnected();

		matchmaker.Send("cancelGame");

		ShowLostConnection();
	}

	public override void BoltShutdownCompleted() {
		base.BoltShutdownCompleted();

		network.networkDelegate = null;

		if (matchmaker.state is MatchmakerWaitForBoltState) {
			(matchmaker.state as MatchmakerWaitForBoltState).BoltShutdownCompleted();
		}

		TransitionTo(new MainMenuState());
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);

		matchmaker.Send("cancelGame");

		ShowLostConnection();
	}

	void ShowLostConnection() {
		HideInGameMenus();
		menu.AddItem(UI.ActivityIndicator("Lost Connection. Returning to Main Menu"));
		menu.Show();

		app.battlefield.SoftReset();
		network.ShutdownBolt();
	}

	// Menu

	public void HideInGameMenus() {
		foreach(var inGameMenu in inGameMenus) {
			inGameMenu.Hide();
		}

		inGameMenus = new List<InGameMenu>();

		app.ResetMenu(); //TODO: Needed?
	}

	public InGameMenu InGameMenuForPlayer(Player player) {
		return inGameMenus.Find(m => m.player == player);
	}

	void ShowInGameMenus() {
		app.ResetMenu();

		inGameMenus = new List<InGameMenu>();

		if (app.battlefield.localPlayers.Count == 0) {
			AddInGameMenu(app.battlefield.player1);
		}
		else {
			foreach (var player in app.battlefield.localPlayers) {
				AddInGameMenu(player);
			}
		}
	}

	void AddInGameMenu(Player player) {
		var inGameMenu = new InGameMenu();
		inGameMenu.playingGameState = this;
		inGameMenu.player = player;
		inGameMenu.menuPlacement = player.localNumber == 2 ? MenuAnchor.TopRight : MenuAnchor.TopLeft;
		inGameMenu.Show();

		player.inGameMenu = inGameMenu;

		inGameMenus.Add(inGameMenu);
	}

	public InGameMenu primaryInGameMenu {
		get {
			if (inGameMenus != null && inGameMenus.Count > 0) {
				return inGameMenus[0];
			}
			else {
				return null;
			}

		}
	}

	public override void ConnectMatchmakerMenu() {
		if (primaryInGameMenu != null) {
			primaryInGameMenu.ConnectMatchmakerMenu();
		}
	}

	public override void DisconnectMatchmakerMenu() {
		if (primaryInGameMenu != null) {
			primaryInGameMenu.DisconnectMatchmakerMenu();
		}
	}
}
