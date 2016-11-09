using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class InGameMenu {
	public PlayingGameState playingGameState;
	public Player player;
	public MenuAnchor menuPlacement;

	public void Show() {
		player.inGameMenu = this;
		App.shared.ResetMenu();
		App.shared.menu.Hide();
		Reset();
	}
		
	public void Hide() {
		if (menu != null) {
			menu.Destroy();
			menu = null;
			player.inGameMenu = null;
			player = null;
		}

		App.shared.menu.Show();
	}

	public void ReadInput () {
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

	UIMenu menu;

	PlayerInputs inputs {
		get {
			return player.inputs;
		}
	}

	void Reset() {
		if (menu != null) {
			menu.Destroy();
		}

		menu = UI.Menu();

		menu.backgroundColor = Color.clear;

		ShowOptionsButton();

		menu.selectsOnShow = false;
		menu.inputs = inputs;
		menu.Show();

		if (App.shared.matchmaker.menu.isOpen) {
			DisconnectMatchmakerMenu();
		}
		else {
			ConnectMatchmakerMenu();
		}
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
		if (player.opponent.inGameMenu != null) {
			if (player.opponent.inGameMenu.isOpen) {
				return;
			}
			else {
				player.opponent.inGameMenu.menu.Hide();
			}
		}
			
		isOpen = true;

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
        menu.AddNewButton().SetText("Close").SetAction(CloseActivated).SetPlayerAction(player.inputs.goBack);

		menu.Focus();
	}

	void Close() {
		if (player.opponent.inGameMenu != null) {
			player.opponent.inGameMenu.menu.Show();
		}

		isOpen = false;
		ShowOptionsButton();
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
		menu.Reset();

		if (App.shared.battlefield.isAiVsAi) { //AIvAI
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
		foreach (Tower _tower in GameObject.FindObjectsOfType<Tower>()) {
			_tower.UpdateHotKeys();
		}
        foreach (UIButton _button in GameObject.FindObjectsOfType<UIButton>()) {
            _button.DoUpdateTextForHotkeys();
        }
	}

	void ChangeCameraActivated() {
		App.shared.cameraController.NextPosition();
	}

	void CloseActivated() {
		Close();
	}

	//Matchmaker Menu

	public void ConnectMatchmakerMenu() {
		/*
		App.shared.Log("ConnectMatchmakerMenu: " + menu, this);
		App.shared.matchmaker.menu.nextMenu = menu;
		App.shared.matchmaker.menu.previousMenu = menu;

		if (menu != null) {
			menu.previousMenu = App.shared.matchmaker.menu;
			menu.nextMenu = App.shared.matchmaker.menu;
		}
		*/
	}

	public void DisconnectMatchmakerMenu() {
		/*
		App.shared.Log("DisconnectMatchmakerMenu: " + menu, this);
		App.shared.matchmaker.menu.nextMenu = null;
		App.shared.matchmaker.menu.previousMenu = null;

		if (menu != null) {
			menu.previousMenu = null;
			menu.nextMenu = null;
		}
		*/
	}
}
