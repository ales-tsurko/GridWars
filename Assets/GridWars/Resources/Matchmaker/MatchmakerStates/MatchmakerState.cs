using UnityEngine;
using System.Collections;

public class MatchmakerState : AppState, MatchmakerDelegate, MatchmakerMenuDelegate {
	public static string MatchmakerSaveAccountNotification = "MatchmakerSaveAccountNotification";


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

	public void PostGame() {
		matchmaker.Send("postGame");
		TransitionTo(new MatchmakerPostedGameState());
	}

	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.matchmakerDelegate = this;
		matchmaker.menu.AddDelegate(this);
		ConfigureMatchmakerMenu();
	}

	public override void WillExit() {
		base.WillExit();
		matchmaker.menu.RemoveDelegate(this);
	}

	// MatchmakerMenuDelegate

	//Called from AppState.EnterFrom
	public virtual void ConfigureMatchmakerMenu() {
		if (matchmaker.menu.isOpen) {
			ConfigureForOpen();
			matchmaker.menu.Focus();
		}
		else {
			ConfigureForClosed();
		}
	}


	public virtual void MatchmakerMenuOpened() {
		//app.Log("MatchmakerMenuOpened", this);
		ConfigureForOpen();
		matchmaker.menu.Focus();
	}

	public virtual void MatchmakerMenuClosed() {
		//app.Log("MatchmakerMenuClosed", this);
		ConfigureForClosed();
	}

	public virtual void ConfigureForOpen() {
		//app.Log("ConfigureForOpen", this);
		matchmaker.menu.SetAnchor(MenuAnchor.MiddleCenter);
		matchmaker.menu.UseDefaultBackgroundColor();
		matchmaker.menu.selectsOnShow = true;
		matchmaker.menu.isInteractible = true;

		if (app.state != null) {
			app.state.ConfigureForOpenMatchmakerMenu();
		}
	}

	public virtual void ConfigureForClosed() {
		//app.Log("ConfigureForClosed", this);
		matchmaker.menu.SetAnchor(MenuAnchor.TopCenter);
		matchmaker.menu.backgroundColor = Color.clear;
		matchmaker.menu.selectsOnShow = false;

		if (app.state != null) {
			app.state.ConfigureForClosedMatchmakerMenu();
		}
	}

	/*
	public virtual void ConnectMatchmakerMenu() {
		if (menu == null || !menu.isActiveAndEnabled) {
			return;
		}
			
		matchmaker.menu.nextMenu = menu;
		matchmaker.menu.previousMenu = menu;

		if (app.menu != null) {
			app.menu.previousMenu = matchmaker.menu;
			app.menu.nextMenu = matchmaker.menu;
		}
	}

	public virtual void DisconnectMatchmakerMenu() {
		matchmaker.menu.nextMenu = null;
		matchmaker.menu.previousMenu = null;
		matchmaker.menu.orientation = MenuOrientation.Vertical;

		if (menu != null) {
			app.menu.previousMenu = null;
			app.menu.nextMenu = null;
		}
	}
	*/

	//MainMenu

	public virtual void MainMenuInternetPvpClicked() {
		matchmaker.menu.isNavigable = true;
		matchmaker.menu.isInteractible = true;
		matchmaker.menu.Open();
	}

	/*

	void DisableMatchmakerMenuInteraction() {
		matchmaker.menu.isNavigable = false;
		matchmaker.menu.isInteractible = false;
	}

	void EnableMatchmakerMenuInteraction() {
		matchmaker.menu.isNavigable = true;
		matchmaker.menu.isInteractible = true;
	}
	*/

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

		if (matchmaker.isConnected) {
			matchmaker.Disconnect();
		}
		else {
			TransitionTo(new MatchmakerDisconnectedState());
		}
	}

	public virtual void MatchmakerReceivedMessage(JSONObject message) {
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
		//app.Log("Unexpected Message: " + name + ": " + data, this);
	}

	public virtual void HandleGamePosted(JSONObject data) {
		account.GamePosted(data);
	}

	public virtual void HandleGameCancelled(JSONObject data) {
		if (data.GetField("host").GetField("id").n == account.id) {
			HandleMyGameCancelled();
		}
		else {
			var client = data.GetField("client");
			if (client != null && (client.GetField("id").n == account.id)) {
				HandleMyGameCancelled();
			}
		}
		account.GameCancelled(data);
	}

	public virtual void HandleMyGameCancelled() {
		app.Log("My Game Cancelled", this);
	}

	public virtual void HandlePlayerConnected(JSONObject data) {
		account.PlayerConnected(data);
	}

	public virtual void HandlePlayerDisconnected(JSONObject data) {
		account.PlayerDisconnected(data);
	}

	public virtual void HandlePlayerBecameUnavailableToPlay(JSONObject data) {
		account.PlayerBecameUnavailableToPlay(data);
	}

	public virtual void HandlePlayerBecameAvailableToPlay(JSONObject data) {
		account.PlayerBecameAvailableToPlay(data);
	}

	public virtual void HandlePlayerChangedScreenName(JSONObject data) {
		account.PlayerChangedScreenName(data);
	}

	public void HandleSaveAccount(JSONObject data) {
		app.notificationCenter.NewNotification()
			.SetName(MatchmakerSaveAccountNotification)
			.SetSender(this)
			.SetData(data)
			.Post();
	}
}
