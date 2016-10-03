using UnityEngine;
using System.Collections;

public class PlayingGameState : NetworkDelegateState {
	//AppState

	bool didDetectEndOfGame;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		network.networkDelegate = this;

		ShowInGameMenu();

		Battlefield.current.StartGame();
	}

	public override void Update() {
		base.Update();

		if (didDetectEndOfGame) {
			return;
		}

		if (battlefield.livingPlayers.Count == 1) {
			var player = battlefield.livingPlayers[0];

			app.ResetMenu();
			menu.SetBackground(Color.black, 0);
			var title = "";
			if (battlefield.localPlayers.Count == 1) {
				if (player.isLocal) {
					title = "Victory!";
				}
				else {
					title = "Defeat!";
				}
			}
			else {
				title = "Player " + player.playerNumber + " is Victorious!";
			}

			menu.AddItem(UI.MenuItem(title, null, MenuItemType.ButtonTextOnly));
			menu.AddItem(UI.MenuItem("Leave Game", LeaveGame));

			menu.Show();

			didDetectEndOfGame = true;
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
		UnityEngine.SceneManagement.SceneManager.LoadScene("BattleField");
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);

		ShowLostConnection();
	}

	// Menu

	void ShowInGameMenu() {
		app.ResetMenu();

		//TODO: something different for shared screen
		menu.AddItem(UI.MenuItem("Concede (" + Keys.CONCEDE.GetKey().ToString() + ")", Concede));
		menu.AddItem(UI.MenuItem("Toggle Hotkeys (" + Keys.TOGGLEKEYS.GetKey().ToString() + ")", ToggleHotkeys));
		menu.AddItem(UI.MenuItem("Change Camera (" + Keys.CHANGECAM.GetKey().ToString() + ")", ChangeCam));
		menu.SetOrientation(MenuOrientation.Horizontal);
		menu.SetAnchor(MenuAnchor.TopCenter);
		menu.SetBackground(Color.black, 0);
		menu.Show();
	}

	void Concede() {
		app.ResetMenu();
		menu.AddItem(UI.MenuItem("Confirm", ReallyConcede));
		menu.AddItem(UI.MenuItem("Cancel", CancelConcede));
		menu.SetOrientation(MenuOrientation.Horizontal);
		menu.SetAnchor(MenuAnchor.TopCenter);
		menu.SetBackground(Color.black, 0);
		menu.Show();
	}

	void ReallyConcede() {
		LeaveGame();
	}

	void CancelConcede() {
		ShowInGameMenu();
	}

	void ToggleHotkeys() {
		app.prefs.keyIconsVisible = !app.prefs.keyIconsVisible;
	}

	void ChangeCam() {
		App.shared.cameraController.NextPosition();
	}

	void ShowLostConnection() {
		if (didDetectEndOfGame) {
			return;
		}

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("Lost Connection.  Returning to Main Menu"));
		menu.Show();

		network.ShutdownBolt();
	}

	void LeaveGame() {
		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("RETURNING TO MAIN MENU"));
		menu.Show();

		network.ShutdownBolt();
	}
}
