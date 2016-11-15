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
		}
		else {
			ConfigureForClosed();
		}
	}


	public virtual void MatchmakerMenuOpened() {
		//app.Log("MatchmakerMenuOpened", this);
		ConfigureForOpen();
	}

	public virtual void MatchmakerMenuClosed() {
		//app.Log("MatchmakerMenuClosed", this);
		ConfigureForClosed();
	}

	public virtual void ConfigureForOpen() {
		//app.Log("ConfigureForOpen", this);
		DisconnectMatchmakerMenu();
		app.menu.Hide();
		matchmaker.menu.SetAnchor(MenuAnchor.MiddleCenter);
		matchmaker.menu.UseDefaultBackgroundColor();
		EnableMatchmakerMenuInteraction();
	}

	public virtual void ConfigureForClosed() {
		//app.Log("ConfigureForClosed", this);
		ConnectMatchmakerMenu();
		matchmaker.menu.SetAnchor(MenuAnchor.TopCenter);
		matchmaker.menu.backgroundColor = Color.clear;
		app.menu.Show();
		if (app.state is MainMenuState) {
			DisableMatchmakerMenuInteraction();
		}
	}

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

	//MainMenu

	public virtual void MainMenuInternetPvpClicked() {
		matchmaker.menu.isNavigable = true;
		matchmaker.menu.isInteractible = true;
		matchmaker.menu.Open();
	}

	public virtual void MainMenuEntered() {
		DisableMatchmakerMenuInteraction();
	}

	public virtual void MainMenuExited() {
		EnableMatchmakerMenuInteraction();
	}

	void DisableMatchmakerMenuInteraction() {
		matchmaker.menu.isNavigable = false;
		matchmaker.menu.isInteractible = false;
	}

	void EnableMatchmakerMenuInteraction() {
		matchmaker.menu.isNavigable = true;
		matchmaker.menu.isInteractible = true;
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

	public void HandleSaveAccount(JSONObject data) {
		app.notificationCenter.NewNotification()
			.SetName(MatchmakerSaveAccountNotification)
			.SetSender(this)
			.SetData(data)
			.Post();
	}
}
