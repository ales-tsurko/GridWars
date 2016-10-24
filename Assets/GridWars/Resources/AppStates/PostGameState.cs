using UnityEngine;
using System.Collections;

public class PostGameState : NetworkDelegateState {
	public Player victoriousPlayer;

	bool requestedRematch;

	//AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		network.networkDelegate = this;

		IsEntering();

		app.ResetMenu();
		menu.SetBackground(Color.black, 0);
		var title = "";
		if (battlefield.localPlayers.Count == 1) {
			if (victoriousPlayer.isLocal) {
				title = "Victory!";
				App.shared.PlayAppSoundNamed("Victory");
			} else {
				title = "Defeat!";
				App.shared.PlayAppSoundNamedAtVolume("Defeat", 0.5f);
			}
		} else {
			title = "Player " + victoriousPlayer.playerNumber + " is Victorious!";
		}

		menu.AddItem(UI.MenuItem(title, null, MenuItemType.ButtonTextOnly));
		if (battlefield.isInternetPVP) {
			menu.AddItem(UI.MenuItem("Request Rematch", RequestRematch));
		} else {
			menu.AddItem(UI.MenuItem("Rematch!", RequestRematch));
		}
		menu.AddItem(UI.MenuItem("Leave Game", LeaveGame));

		menu.Show();
	}

	void IsEntering() {
		Object.FindObjectOfType<CameraController>().StartOrbit();
	}

	void IsExiting() {
		Object.FindObjectOfType<CameraController>().EndOrbit();
	}

	// Network

	public override void ZeusDisconnected() {
		base.ZeusDisconnected();

		ShowLostConnection();
		IsExiting();
	}

	public override void BoltShutdownCompleted() {
		base.BoltShutdownCompleted();

		network.networkDelegate = null;

		TransitionTo(new MainMenuState());
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);

		ShowLostConnection();
		IsExiting();
	}

	public override void ReceivedRematchRequest() {
		base.ReceivedRematchRequest();

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("Opponent Requests a Rematch"));
		menu.AddItem(UI.MenuItem("Accept", AcceptRematch));
		menu.AddItem(UI.MenuItem("Decline", LeaveGame));
		menu.Show();
	}

	public override void ReceivedAcceptRematch() {
		base.ReceivedAcceptRematch();

		if (BoltNetwork.isServer) {
			App.shared.StartCoroutine(ServerReceivedAcceptRematch());
		}
		else {
			TransitionTo(new PlayingGameState());
		}
	}

	IEnumerator ServerReceivedAcceptRematch() {
		battlefield.SoftReset();
		while (battlefield.livingPlayers.Count > 0) {
			yield return null;
		}

		TransitionTo(new PlayingGameState());
	}

	// Menus

	void ShowLostConnection() {
		string prefix = "";
		if (requestedRematch) {
			prefix = "Opponent Declined. ";
		}

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator(prefix + "Returning to Main Menu"));
		menu.Show();
		IsExiting();

		app.battlefield.HardReset();
		network.ShutdownBolt();
	}

	// LeaveGame

	void LeaveGame() {
		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("RETURNING TO MAIN MENU"));
		menu.Show();
		IsExiting();


		if (BoltNetwork.isRunning) {
			app.battlefield.HardReset();
			network.ShutdownBolt();
		}
		else {
			BoltShutdownCompleted();
		}
	}

	//Rematch

	void RequestRematch() {

		if (!battlefield.isInternetPVP) {
			battlefield.SoftReset();
			TransitionTo(new PlayingGameState());
			return;
		}

		RequestRematchEvent.Create(Bolt.GlobalTargets.Others, Bolt.ReliabilityModes.ReliableOrdered).Send();
		app.Log("RequestRematchEvent.Send", this);

		//App.shared.PlayAppSoundNamedAtVolume("Rematch", 0.3f); // want to play this until menu is removed

		requestedRematch = true;

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("WAITING FOR RESPONSE"));
		menu.AddItem(UI.MenuItem("Cancel", LeaveGame), true);
		menu.Show();
	}

	void AcceptRematch() {
		if (BoltNetwork.isServer) {
			App.shared.StartCoroutine(ServerAcceptRematch());
		}
		else {
			ClientAcceptRematch();
		}
	}

	IEnumerator ServerAcceptRematch() {
		battlefield.SoftReset();
		while (battlefield.livingPlayers.Count > 0) {
			yield return null;
		}

		SendAcceptRematchEvent();
	}

	void ClientAcceptRematch() {
		SendAcceptRematchEvent();
	}

	void SendAcceptRematchEvent() {
		AcceptRematchEvent.Create(Bolt.GlobalTargets.Others, Bolt.ReliabilityModes.ReliableOrdered).Send();
		app.Log("AcceptRematchEvent.Send", this);

		TransitionTo(new PlayingGameState());
	}
}
