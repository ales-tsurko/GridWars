using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayingGameState : AppState {
	List<InGameMenu> inGameMenus;

	//AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		Object.FindObjectOfType<CameraController>().EndOrbit();

		battlefield.canCheckGameOver = false;

		//do this before ShowInGameMenu
		if (battlefield.isInternetPVP) {
			battlefield.player1.isLocal = BoltNetwork.isServer;
			battlefield.player2.isLocal = BoltNetwork.isClient;
		}

		ShowInGameMenus();

		App.shared.SoundtrackNamed("MenuBackgroundMusic").FadeOut();

		App.shared.PlayAppSoundNamed("GameStart");

		menu.Hide();

		battlefield.SoftReset();

		app.StartCoroutine(StartGameAfterBattlefieldEmpty());
	}

	IEnumerator StartGameAfterBattlefieldEmpty() {
		while (BoltNetwork.isServer && battlefield.livingPlayers.Count > 0) {
			yield return null;
		}

		battlefield.StartGame();
	}

	public override void WillExit() {
		base.WillExit();

		menu.Show();

		HideInGameMenus();
	}

	public override void Update() {
		base.Update();

		if (BoltNetwork.isServer && battlefield.canCheckGameOver && battlefield.livingPlayers.Count == 1) {
			EndGame(battlefield.livingPlayers[0]);
		}
		else {
			foreach (var inGameMenu in inGameMenus) {
				inGameMenu.ReadInput();
			}
		}
	}

	public void EndGame(Player victor) {
		if (battlefield.isInternetPVP) {
			(matchmaker.state as MatchmakerPlayingGameState).EndGame(victor);
		}
		GameEnded(victor);
	}

	public void GameEnded(Player victor) {
		var state = new PostGameState();
		state.victoriousPlayer = victor;
		TransitionTo(state);
	}

	public void GameCancelled() {
		if (battlefield.isInternetPVP) {
			network.Reset();
			ShowLostConnection();
		}
	}

	//NetworkDelegate

	/*
	public override void ZeusDisconnected() {
		base.ZeusDisconnected();

		//(matchmaker.state as MatchmakerPlayingGameState).EndGame(victor);
		matchmaker.Send("cancelGame");

		ShowLostConnection();
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);

		matchmaker.Send("cancelGame");
		ShowLostConnection();
	}
	*/

	void ShowLostConnection() {
		HideInGameMenus();

		menu.AddNewText().SetText("Lost Connection");
		menu.AddNewButton().SetText("Leave").SetAction(Leave);
		menu.Show();
	}

	public void Leave() {
		TransitionTo(new MainMenuState());
	}

	// In Game Menu

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

	// Matchmaker

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
