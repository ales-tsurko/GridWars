using UnityEngine;
using System.Collections;

public class MatchmakerState : AppState, MatchmakerDelegate, MatchmakerMenuDelegate {

	public Account account {
		get {
			return app.account;
		}
	}

	public Game game {
		get {
			return account.game;
		}
	}

	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Show();
		matchmaker.matchmakerDelegate = this;
	}

	// MatchmakerMenuDelegate

	public override void ConfigureMatchmakerMenu() {
		base.ConfigureMatchmakerMenu();

		if (matchmaker.menu.isOpen) {
			MatchmakerMenuOpened();
		}
		else {
			MatchmakerMenuClosed();
		}
	}

	public override void MatchmakerMenuOpened() {
		//app.Log("MatchmakerMenuOpened", this);
	}

	public override void MatchmakerMenuClosed() {
		//app.Log("MatchmakerMenuClosed", this);
	}

	//MatchmakerDelegate

	public virtual void MatchmakerConnected() {
		app.Log("MatchmakerConnected", this);
	}


	public virtual void MatchmakerDisconnected() {
		app.Log("MatchmakerDisconnected", this);

		TransitionTo(new MatchmakerDisconnectedState());
	}

	public virtual void MatchmakerErrored() {
		app.Log("MatchmakerErrored", this);

		matchmaker.Disconnect();

		TransitionTo(new MatchmakerDisconnectedState());
	}

	public virtual void MatchmakerReceivedMessage(JSONObject message) {
		app.Log("MatchmakerReceivedMessage: " + message.ToString(), this);

		HandleMessage(message.GetField("name").str, message.GetField("data"));
	}

	public virtual void HandleMessage(string name, JSONObject data) {
		var methodName = "Handle" + name.Capitalized();
		var method = this.GetType().GetMethod(methodName);
		if (method == null) {
			this.HandleUnexpectedMessage(name, data);
		}
		else {
			app.Log(methodName, this);
			method.Invoke(this, new object[]{ data });
		}
	}

	protected void HandleUnexpectedMessage(string name, JSONObject data) {
		app.Log("Unexpected Message: " + name + ": " + data, this);
	}

	public virtual void HandleGamePosted(JSONObject data) {
	}

	public virtual void HandlePlayerConnected(JSONObject data) {
		account.PlayerConnected(data);
	}

	public virtual void HandlePlayerDisconnected(JSONObject data) {
		account.PlayerDisconnected(data);
	}
}
