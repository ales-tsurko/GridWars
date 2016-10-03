using UnityEngine;
using System.Collections;

public class WaitForBoltState : NetworkDelegateState {

	//AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		network.networkDelegate = this;
		BoltLauncher.StartSinglePlayer();
	}

	// NetworkDelegate

	public override void BoltStartDone() {
		base.BoltStartDone();

		TransitionTo(new PlayingGameState());
	}
}
