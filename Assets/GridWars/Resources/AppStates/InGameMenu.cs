﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class InGameMenu {
	public static string InGameMenuOpenedNotification = "InGameMenuOpenedNotification";
	public static string InGameMenuClosedNotification = "InGameMenuClosedNotification";

	public PlayingGameState playingGameState;
	public Player player;
	public MenuAnchor menuPlacement;
		
	public void Destroy() {
		if (menu != null) {
			menu.Destroy();
			menu = null;
			player.inGameMenu = null;
			player = null;
		}
	}

	public void Show() {
		menu.Show();
	}

	public void Hide() {
		menu.Hide();
	}

	public void ReadInput () {
		if (menu == null) {
			return;
		}

		if (inputs.toggleMenu.WasPressed) {
			ToggleMenuActivated();
		}

		if (inputs.concede.WasPressed) {
			ConcedeActivated();
		}

		if (inputs.toggleHotkeys.WasPressed) {
			HotkeysActivated();
		}

		if (inputs.nextCamera.WasPressed) {
			ChangeCameraActivated();
		}
	}

	public UIMenu menu;

	PlayerInputs inputs {
		get {
			return player.inputs;
		}
	}

	public void Setup() {
		player.inGameMenu = this;
		App.shared.ResetMenu();
		App.shared.menu.Hide();

		menu = UI.Menu();

		menu.backgroundColor = Color.clear;

		ShowOptionsButton();

		menu.selectsOnShow = false;
		menu.inputs = inputs;
		menu.Show();
	}

	void ToggleMenuActivated() {
		if (isOpen) {
			Close();
		}
		else {
			Open();
		}
	}

	public bool isOpen = false;

	void Open() {
		if (isOpen) {
			return;
		}

		if (player.opponent.inGameMenu != null) {
			if (player.opponent.inGameMenu.isOpen) {
				return;
			}
			else {
				player.opponent.inGameMenu.menu.Hide();
			}
		}

		isOpen = true;

		App.shared.matchmaker.menu.Close();

		menu.Reset();
		menu.UseDefaultBackgroundColor();
		menu.anchor = MenuAnchor.MiddleCenter;

		string text;
		if (App.shared.battlefield.isAiVsAi) { //AIvAI
			text = "Quit";
		}
		else {
			text = "Concede";
		}

        menu.AddNewButton().SetText(text).SetAction(ConcedeActivated).SetPlayerAction(player.inputs.concede);
        menu.AddNewButton().SetText("Hotkeys").SetAction(HotkeysActivated).SetPlayerAction(player.inputs.toggleHotkeys);
        menu.AddNewButton().SetText("Change Camera").SetAction(ChangeCameraActivated).SetPlayerAction(player.inputs.nextCamera);
        menu.AddNewButton().SetText("Close").SetAction(CloseActivated);

		menu.Focus();

		App.shared.notificationCenter.NewNotification()
			.SetName(InGameMenuOpenedNotification)
			.SetSender(this)
			.Post();
	}

	public void Close() {
		if (!isOpen) {
			return;
		}

		if (player.opponent.inGameMenu != null) {
			player.opponent.inGameMenu.Show();
		}

		isOpen = false;
		ShowOptionsButton();

		App.shared.notificationCenter.NewNotification()
			.SetName(InGameMenuClosedNotification)
			.SetSender(this)
			.Post();
	}

	void ShowOptionsButton() {
		menu.Reset();
		menu.backgroundColor = Color.clear;
		menu.anchor = menuPlacement;
        menu.AddNewButton().SetText("Options").SetAction(OptionsActivated).SetPlayerAction(player.inputs.toggleMenu);

	}

	void OptionsActivated() {
		Open();
	}

	void ConcedeActivated() {
		Open();

		menu.Reset();
		menu.anchor = MenuAnchor.MiddleCenter;

		menu.AddNewText().SetText("Are you sure?");

		if (App.shared.battlefield.isAiVsAi && !App.shared.battlefield.player1.isTutorialMode) { //AIvAI
			menu.AddNewButton().SetText("Leave").SetAction(ConfirmConcedeActivated);
			menu.AddNewButton().SetText("Rematch").SetAction(RematchActivated);
		}
		else {
			menu.AddNewButton().SetText("Confirm").SetAction(ConfirmConcedeActivated);
		}

		menu.AddNewButton().SetText("Cancel").SetAction(CancelConcedeActivated);

		menu.Focus();
	}

	void ConfirmConcedeActivated() {
		if (App.shared.battlefield.isAiVsAi) {
			playingGameState.Leave();
		}
		else {
			playingGameState.EndGame(player.opponent);
		}
	}

	void RematchActivated() {
		playingGameState.RestartGame();
	}

	void CancelConcedeActivated() {
		CloseActivated();
	}

	void HotkeysActivated() {
		App.shared.prefs.keyIconsVisible = !App.shared.prefs.keyIconsVisible;
	}

	void ChangeCameraActivated() {
		App.shared.cameraController.NextPosition();
	}

	void CloseActivated() {
		Close();
	}

	//Matchmaker Menu

	public void ConnectMatchmakerMenu() {
		App.shared.Log("ConnectMatchmakerMenu", this);

		App.shared.matchmaker.menu.nextMenu = menu;
		App.shared.matchmaker.menu.previousMenu = menu;

		if (menu != null) {
			menu.previousMenu = App.shared.matchmaker.menu;
			menu.nextMenu = App.shared.matchmaker.menu;
		}
	}

	public void DisconnectMatchmakerMenu() {
		App.shared.Log("DisconnectMatchmakerMenu", this);

		App.shared.matchmaker.menu.nextMenu = null;
		App.shared.matchmaker.menu.previousMenu = null;

		if (menu != null) {
			menu.previousMenu = null;
			menu.nextMenu = null;
		}
	}
}
