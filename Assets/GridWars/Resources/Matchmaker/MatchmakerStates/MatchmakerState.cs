using UnityEngine;
using System.Collections;

public class MatchmakerState : AppState, MatchmakerDelegate, MatchmakerMenuDelegate {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.matchmakerDelegate = this;
		matchmaker.menu.theDelegate = this;

		if (matchmaker.menu.isOpen) {
			MatchmakerMenuOpened();
		}
		else {
			MatchmakerMenuClosed();
		}
	}

	public virtual void MatchmakerMenuOpened() {
		
	}

	public virtual void MatchmakerMenuClosed() {

	}

	public void MatchmakerConnected() {
		app.Log("MatchmakerConnected", this);
	}


	public virtual void MatchmakerDisconnected() {
		app.Log("MatchmakerDisconnected", this);
	}

	public virtual void MatchmakerErrored() {
		app.Log("MatchmakerErrored", this);
	}

	public virtual void MatchmakerReceivedMessage(JSONObject message) {
		app.Log("MatchmakerReceivedMessage: " + message.ToString(), this);
	}

	public virtual void MatchmakerReceivedHost(string gameId) {
	}

	public virtual void MatchmakerReceivedJoin(string gameId) {
	}

	public virtual void MatchmakerReceivedVersion(string version) {
	}
}
