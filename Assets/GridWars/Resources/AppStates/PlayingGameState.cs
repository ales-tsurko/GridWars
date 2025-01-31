﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class PlayingGameState : AppState {
	List<InGameMenu> inGameMenus;
	UIMenu levelMenu;

	//AppState

	public override UIMenu[] focusableMenus {
		get {
			var openMenu = inGameMenus.Find(m => m.isOpen);
			if (openMenu == null) {
				return new UIMenu[]{ menu };
			}
			else {
				return new UIMenu[]{ openMenu.menu, matchmaker.menu };
			}
		}
	}

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		Object.FindObjectOfType<CameraController>().GameStarted();

		battlefield.canCheckGameOver = false;

		//do this before ShowInGameMenu
		if (battlefield.isInternetPVP) {
			battlefield.player1.isLocal = BoltNetwork.isServer;
			battlefield.player2.isLocal = BoltNetwork.isClient;
			battlefield.player1.npcModeOn = false;
			battlefield.player2.npcModeOn = false;
		}

		app.ResetMenu();

		inGameMenus = new List<InGameMenu>();

		if (app.battlefield.localPlayers.Count == 0) {
			AddInGameMenu(app.battlefield.player1);
		}
		else {
			foreach (var player in app.battlefield.localPlayers) {
				AddInGameMenu(player);
			}

			if (app.battlefield.isPvsAI()) {
				AddLevelMenu();
			}
		}

		//primaryInGameMenu.menu.Focus();

		App.shared.SoundtrackNamed("MenuBackgroundMusic").FadeOut();

		App.shared.PlayAppSoundNamed("GameStart");

		menu.Hide();

		matchmaker.menu.Close();
		ConnectMatchmakerMenu();

		battlefield.SoftReset();

		app.StartCoroutine(StartGameAfterBattlefieldEmpty());
        app.account.LogEvent("PlayingGame");
	}

	IEnumerator StartGameAfterBattlefieldEmpty() {
		while (BoltNetwork.isServer && !battlefield.isEmpty) {
			yield return null;
		}

		battlefield.StartGame();
	}

	public override void WillExit() {
		base.WillExit();

		menu.Show();

		if (levelMenu != null) {
			levelMenu.Destroy();
			levelMenu = null;
		}

		DestroyInGameMenus();
	}

	public override void Update() {
		base.Update();

		//TODO: what about a tie?
		if (BoltNetwork.isServer && battlefield.canCheckGameOver && 
			battlefield.GameOver()) {
			Player winner = battlefield.livingPlayers[0];
			EndGame(winner);

		}
		else {
			foreach (var inGameMenu in inGameMenus) {
				inGameMenu.ReadInput();
			}
		}
	}

	public void RestartGame() {
		TransitionTo(new PlayingGameState());
	}

	public void EndGame(Player victor) {
		if (battlefield.isInternetPVP) {
			(matchmaker.state as MatchmakerPlayingGameState).EndGame(victor);
		}
		GameEnded(victor);
	}

	public void GameEnded(Player victor) {
		if (battlefield.isAiVsAi) {
			TransitionTo(new PlayingGameState());
		}
		else {
			var state = new PostGameState();
			state.victoriousPlayer = victor;
			TransitionTo(state);
		}
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
		DestroyInGameMenus();

		menu.AddNewText().SetText("Lost Connection");
		menu.AddNewButton().SetText("Leave").SetAction(Leave);
		menu.Show();
	}

	public void Leave() {
		TransitionTo(new MainMenuState());
	}

	// In Game Menu

	public void DestroyInGameMenus() {
		DisconnectMatchmakerMenu();

		foreach(var inGameMenu in inGameMenus) {
			inGameMenu.Destroy();
		}

		inGameMenus = new List<InGameMenu>();
	}

	public InGameMenu InGameMenuForPlayer(Player player) {
		return inGameMenus.Find(m => m.player == player);
	}

	void AddInGameMenu(Player player) {
		var inGameMenu = new InGameMenu();
		inGameMenu.playingGameState = this;
		inGameMenu.player = player;
		inGameMenu.menuPlacement = player.localNumber == 2 ? MenuAnchor.TopRight : MenuAnchor.TopLeft;
		inGameMenu.Setup();

		inGameMenus.Add(inGameMenu);
	}

	void AddLevelMenu() {
		levelMenu = UI.Menu();

		levelMenu.anchor = MenuAnchor.TopRight;
		levelMenu.AddNewText().text = "Level " + battlefield.npcLevel + " (NPC POWER " + string.Format("{0:F2}", battlefield.localPlayer1.opponent.npcHandicap) + ")";

		levelMenu.Show();
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

	//matchmaker

	public override void ConfigureForOpenMatchmakerMenu() {
		//App.shared.Log("ConfigureForOpenMatchmakerMenu", this);
		//don't call base

		if (primaryInGameMenu != null) {
			primaryInGameMenu.DisconnectMatchmakerMenu();
		}


		foreach (var menu in inGameMenus) {
			menu.Close();
		}

		//inGameMenu.Close shows opponents menu
		foreach (var menu in inGameMenus) {
			menu.Hide();
		}
	}

	public override void ConfigureForClosedMatchmakerMenu() {
		//App.shared.Log("ConfigureForClosedMatchmakerMenu", this);
		//don't call base
		ConnectMatchmakerMenu();
		foreach (var menu in inGameMenus) {
			menu.Show();
		}
	}

	public override void ConnectMatchmakerMenu() {
		//don't call base
		if (primaryInGameMenu != null) {
			primaryInGameMenu.ConnectMatchmakerMenu();
		}
	}

	public override void DisconnectMatchmakerMenu() {
		//don't call base
		if (primaryInGameMenu != null) {
			primaryInGameMenu.DisconnectMatchmakerMenu();
		}
	}
}
