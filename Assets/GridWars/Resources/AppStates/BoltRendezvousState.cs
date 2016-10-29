using UnityEngine;
using System.Collections;

public class BoltRendezvousState : NetworkDelegateState, MatchmakerDelegate {

	//public
	public string gameId;

	public virtual void Cancel() {
		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("RETURNING TO MAIN MENU"));
		menu.Show();

		matchmaker.matchmakerDelegate = null;
		matchmaker.Disconnect();

		postBoltShutdownState = new MainMenuState();

		network.ShutdownBolt();
	}

	//private
	AppState postBoltShutdownState;

	void ReturnToMainMenu() {
		network.networkDelegate = null;
		TransitionTo(new MainMenuState());
	}


	//AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		network.networkDelegate = this;
		matchmaker.matchmakerDelegate = this;
	}

	//NetworkDelegate

	public override void BoltStartDone() {
		base.BoltStartDone();

		Bolt.Zeus.Connect(UdpKit.UdpEndPoint.Parse(network.zeusEndpoint));
		app.Log("Bolt.Zeus.Connect", this);
	}

	public override void BoltStartFailed() {
		base.BoltStartFailed();

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("NETWORK ERROR! RETURNING TO MAIN MENU"));
		var timer = app.timerCenter.NewTimer();
		timer.action = ReturnToMainMenu;
		timer.timeout = 2f;
	}

	public override void ZeusDisconnected() {
		base.ZeusDisconnected();

		network.ShutdownBolt();
	}

	public override void Connected(BoltConnection connection) {
		base.Connected(connection);

		TransitionTo(new PlayingGameState());
	}

	public override void BoltShutdownCompleted() {
		base.BoltShutdownCompleted();

		network.networkDelegate = null;
		TransitionTo(postBoltShutdownState);
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);

		postBoltShutdownState = new OldMatchmakerState();
		network.ShutdownBolt();
	}

	//Matchmaker

	public void MatchmakerConnected() {
		
	}

	public void MatchmakerDisconnected() {
		postBoltShutdownState = new OldMatchmakerState();
		network.ShutdownBolt();
	}

	public void MatchmakerErrored() {
		postBoltShutdownState = new OldMatchmakerState();
		matchmaker.Disconnect();
	}

	public void MatchmakerReceivedMessage(JSONObject message) {
		
	}

	public void MatchmakerReceivedHost(string gameId) {
		throw new System.Exception("Unexpected MatchmakerReceivedHost");
	}

	public void MatchmakerReceivedJoin(string gameId) {
		throw new System.Exception("Unexpected MatchmakerReceivedJoin");
	}

	public void MatchmakerReceivedVersion(string version) {
		throw new System.Exception("Unexpected MatchmakerReceivedVersion");
	}
}
