using UnityEngine;
using System.Collections;

public class MatchmakerWaitForPeerState : MatchmakerNetworkDelegateState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Open();

		network.networkDelegate = this;

		if (app.state is PlayingGameState || app.state is PostGameState) {
			battlefield.SoftReset();
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

	public override void MatchmakerMenuOpened() {
		base.MatchmakerMenuOpened();

		matchmaker.menu.Reset();

		matchmaker.menu.AddNewIndicator().SetText("Connecting to opponent.");

		matchmaker.menu.AddNewButton()
			.SetText("Leave")
			.SetAction(CancelAndLeave);

		matchmaker.menu.Show();
	}
		
	void CancelAndLeave() {
		matchmaker.Send("cancelGame");
		Leave();
	}

	void Leave() {
		network.Reset();
		TransitionTo(new MatchmakerPostAuthState());
	}

	// MatchmakerDelegate

	public void HandleGameCancelled(JSONObject data) {
		if (data.GetField("id").str == app.account.game.id) {
			matchmaker.menu.Reset();
			matchmaker.menu.AddNewText()
				.SetText("Opponent left.");
			matchmaker.menu.AddNewButton()
				.SetText("Leave")
				.SetAction(Leave);
			matchmaker.menu.Show();
		}
	}

	public void HandleStartGame(JSONObject data) {
		TransitionTo(new MatchmakerPlayingGameState());
	}
}
