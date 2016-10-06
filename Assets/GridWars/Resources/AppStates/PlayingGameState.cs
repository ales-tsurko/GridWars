using UnityEngine;
using System.Collections.Generic;

public class PlayingGameState : NetworkDelegateState {
	bool didHardReset = false;

	//AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		battlefield.canCheckGameOver = false;

		network.networkDelegate = this;
		matchmaker.matchmakerDelegate = null;
		if (matchmaker.isConnected) {
			matchmaker.Disconnect();
		}

		ShowInGameMenu();

		if (battlefield.isInternetPVP) {
			battlefield.PlayerNumbered(1).isLocal = BoltNetwork.isServer;
			battlefield.PlayerNumbered(2).isLocal = BoltNetwork.isClient;
		}

		Battlefield.current.StartGame();
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
			if (Keys.CHANGECAM.Pressed()){
				ChangeCam();
			}
			if (Keys.CONCEDE.Pressed()) {
				Concede();
			}
			if (Keys.TOGGLEKEYS.Pressed()){
				ToggleHotkeys();
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
		state.victoriousPlayer = battlefield.localPlayer;

		TransitionTo(state);
	}

	void ShowLostConnection() {
		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("Lost Connection.  Returning to Main Menu"));
		menu.Show();

		app.battlefield.HardReset();
		didHardReset = true;
		network.ShutdownBolt();
	}

	// Menu

	void ShowInGameMenu() {
		app.ResetMenu();

		//TODO: something different for shared screen
		menu.AddItem(UI.MenuItem("Concede (" + Keys.CONCEDE.GetKey().ToString() + ")", Concede));
		menu.AddItem(UI.MenuItem("Toggle Hotkeys (" + Keys.TOGGLEKEYS.GetKey().ToString() + ")", ToggleHotkeys));
		menu.AddItem(UI.MenuItem("Change Camera (" + Keys.CHANGECAM.GetKey().ToString() + ")", ChangeCam));
		menu.SetOrientation(MenuOrientation.Horizontal);
		menu.SetAnchor(MenuAnchor.TopLeft);
		menu.SetBackground(Color.black, 0);
		menu.Show();
	}

	// Concede

	void Concede() {
		app.ResetMenu();
		menu.AddItem(UI.MenuItem("Confirm", ReallyConcede));
		menu.AddItem(UI.MenuItem("Cancel", CancelConcede));
		menu.SetOrientation(MenuOrientation.Horizontal);
		menu.SetAnchor(MenuAnchor.TopLeft);
		menu.SetBackground(Color.black, 0);
		menu.Show();
	}

	void ReallyConcede() {
		var state = new PostGameState();
		state.victoriousPlayer = battlefield.localPlayer.opponent;

		ConcedeEvent.Create(Bolt.GlobalTargets.Others, Bolt.ReliabilityModes.ReliableOrdered).Send();

		TransitionTo(state);
	}

	void CancelConcede() {
		ShowInGameMenu();
	}

	// Hotkeys

	void ToggleHotkeys() {
		app.prefs.keyIconsVisible = !app.prefs.keyIconsVisible;
	}


	//Camera

	void ChangeCam() {
		App.shared.cameraController.NextPosition();
	}
}
