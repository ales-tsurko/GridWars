using UnityEngine;
using System.Collections.Generic;

public class PlayingGameState : NetworkDelegateState {
	bool didHardReset = false;
	List<InGameMenu> inGameMenus;

	//AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		Object.FindObjectOfType<CameraController>().EndOrbit();

		battlefield.canCheckGameOver = false;

		network.networkDelegate = this;
		matchmaker.matchmakerDelegate = null;
		if (matchmaker.isConnected) {
			matchmaker.Disconnect();
		}

		//do this before ShowInGameMenu
		if (battlefield.isInternetPVP) {
			battlefield.player1.isLocal = BoltNetwork.isServer;
			battlefield.player2.isLocal = BoltNetwork.isClient;
		}

		ShowInGameMenus();

		Battlefield.current.StartGame();

		App.shared.SoundtrackNamed("MenuBackgroundMusic").FadeOut();

		App.shared.PlayAppSoundNamed("GameStart");

	}

	public override void WillExit() {
		base.WillExit();

		HideInGameMenus();
	}

	public override void Update() {
		base.Update();

		if (didHardReset) {
			return;
		}

		if (battlefield.canCheckGameOver && battlefield.livingPlayers.Count == 1) {
			var state = new PostGameState();
			state.victoriousPlayer = battlefield.livingPlayers[0];
			TransitionTo(state);
		}
		else {
			foreach (var inGameMenu in inGameMenus) {
				inGameMenu.ReadInput();
			}
		}
	}

	//NetworkDelegate

	public override void ZeusDisconnected() {
		base.ZeusDisconnected();

		ShowLostConnection();
	}

	public override void BoltShutdownCompleted() {
		base.BoltShutdownCompleted();

		network.networkDelegate = null;

		TransitionTo(new MainMenuState());
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);

		ShowLostConnection();
	}

	public override void ReceivedConcede() {
		base.ReceivedConcede();

		var state = new PostGameState();
		state.victoriousPlayer = battlefield.localPlayer1;

		TransitionTo(state);
	}

	void ShowLostConnection() {
		HideInGameMenus();
		menu.AddItem(UI.ActivityIndicator("Lost Connection. Returning to Main Menu"));
		menu.Show();

		app.battlefield.HardReset();
		didHardReset = true;
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
}
