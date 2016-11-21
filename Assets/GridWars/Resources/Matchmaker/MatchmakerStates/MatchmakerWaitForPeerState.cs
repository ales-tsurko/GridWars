using UnityEngine;
using System.Collections;

public class MatchmakerWaitForPeerState : MatchmakerNetworkDelegateState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Open();

		if (app.state is PlayingGameState || app.state is PostGameState) {
			network.ShutdownBolt();
		}
		else {
			StartBolt();
		}
	}

	void RestartBolt() {
	}

	protected virtual void StartBolt() {
		
	}

	// NetworkDelegate

	public override void BoltStartDone() {
		base.BoltStartDone();

		ConnectToZeus();
	}

	public override void BoltStartFailed() {
		base.BoltStartFailed();

		StartBolt();
	}

	public override void ZeusConnectFailed() {
		base.ZeusConnectFailed();

		ConnectToZeus();
	}

	public override void BoltShutdownCompleted() {
		base.BoltShutdownCompleted();

		StartBolt();
	}

	void ConnectToZeus() {
		Bolt.Zeus.Connect(UdpKit.UdpEndPoint.Parse(network.zeusEndpoint));
		app.Log("Bolt.Zeus.Connect", this);
	}


	// MatchmakerMenuDelegate

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();

		matchmaker.menu.Reset();

		matchmaker.menu.AddNewIndicator().SetText("Connecting to opponent.");

		matchmaker.menu.AddNewButton()
			.SetText("Leave")
			.SetAction(CancelAndLeave);
	}
		
	void CancelAndLeave() {
		matchmaker.Send("cancelGame");
		Leave();
	}

	void Leave() {
		network.Reset();
		TransitionTo(new MatchmakerPostAuthState());

		if (app.state is PlayingGameState || app.state is PostGameState) {
			app.state.TransitionTo(new MainMenuState());
		}
	}

	// MatchmakerDelegate

	public override void MatchmakerDisconnected() {
		base.MatchmakerDisconnected();

		Leave();
	}

	public override void MatchmakerErrored() {
		base.MatchmakerErrored();

		Leave();
	}

	public override void HandleMyGameCancelled() {
		base.HandleMyGameCancelled();

		OpponentLeft();
	}

	public void HandleStartGame(JSONObject data) {
		TransitionTo(new MatchmakerPlayingGameState());
	}

	void OpponentLeft() {
		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText()
			.SetText("Opponent left.");
		matchmaker.menu.AddNewButton()
			.SetText("Leave")
			.SetAction(Leave);
		
		matchmaker.menu.Focus();
	}
}
