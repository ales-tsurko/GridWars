﻿using UnityEngine;
using System.Collections;

public class MatchmakerDisconnectedState : MatchmakerState {

	// AppState

	bool isConnecting = false;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		MainMenuState mainMenuState = app.state as MainMenuState;
		if (mainMenuState != null) {
			mainMenuState.MatchmakerDisconnected();
		}

		Connect();
	}

	void Connect() {
		if (!isConnecting) {
			app.account.ResetPlayerList();
			//TODO: countdown?
			matchmaker.Connect();
			isConnecting = true;
		}
	}

	//app.state

	//MatchmakerDelegate

	public override void MatchmakerConnected() {
		TransitionTo(new MatchmakerPreAuthState());
	}

	public override void MatchmakerDisconnected() {
		//base.MatchmakerDisconnected(); Don't call base

		app.Log("MatchmakerDisconnected", this);

		app.StartCoroutine(ReconnectCoroutine());
	}

	public override void MatchmakerErrored() {
		//base.MatchmakerErrored(); Don't call base

		app.Log("MatchmakerErrored", this);

		app.StartCoroutine(ReconnectCoroutine());
	}

	IEnumerator ReconnectCoroutine() {
		yield return new WaitForSeconds(0.25f);
		Connect();
	}

	// MatchmakerMenuDelegate

	public override void MatchmakerMenuOpened() {
		base.MatchmakerMenuOpened();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText()
			.SetText("Unable to connect to the server.\n\nInternet matches disabled.");
		matchmaker.menu.AddNewButton()
			.SetText("Close")
			.SetAction(matchmaker.menu.Close)
			.SetIsBackItem(true);
	}

	public override void MatchmakerMenuClosed() {
		base.MatchmakerMenuClosed();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText("Offline")
			.SetTextColor(Color.red)
			.SetAction(matchmaker.menu.Open)
			.UseAlertStyle();
	}
}
