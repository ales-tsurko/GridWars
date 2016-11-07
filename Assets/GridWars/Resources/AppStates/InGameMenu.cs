using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class InGameMenu {
	public PlayingGameState playingGameState;
	public Player player;
	public MenuAnchor menuPlacement;

	public bool hasFocus {
		get {
			return menu.hasFocus;
		}
	}

	public void Show() {
		App.shared.ResetMenu();

		Reset();
	}

	public void Hide() {
		if (menu != null) {
			App.shared.Log("Hide menu", this);
			menu.Destroy();
			menu = null;
			player.inGameMenu = null;
			player = null;
		}
	}

	public void ReadInput () {
		concedeItem.ReadInput();

		if (hotkeysItem != null) {
			hotkeysItem.ReadInput();
		}

		if (previousCameraItem != null) {
			previousCameraItem.ReadInput();
		}

		if (nextCameraItem != null) {
			nextCameraItem.ReadInput();
		}

		if (firstPersonCameraItem != null) {
			firstPersonCameraItem.ReadInput();
		}

		/*if (inputs.toggleMenu.WasPressed && !App.shared.cameraController.isInFirstPersonMode) {
			if (menu.hasFocus) {
				menu.LoseFocus();
			}
			else {
                MonoBehaviour.FindObjectOfType<CameraController>().menuHasFocus = true;
				menu.SelectNextItem();
			}
		}*/
	}

	UIMenu menu;
	InGameMenuItem concedeItem;
	InGameMenuItem hotkeysItem;
	InGameMenuItem nextCameraItem;
	InGameMenuItem previousCameraItem;
	InGameMenuItem firstPersonCameraItem;

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

		string title;

		//TODO: detect which player conceded in shared screen pvp
		if (App.shared.battlefield.isAiVsAi) { //AIvAI
			title = "Quit";
		}
		else {
			title = "Concede";
		}
        menu.isNavigable = false;
		concedeItem = new InGameMenuItem();
		concedeItem.title = title;
		concedeItem.inGameMenu = this;
		concedeItem.playerAction = inputs.concede;
		concedeItem.menuAction = HandleConcede;
		menu.AddItem(concedeItem.menuItem);

		if (!App.shared.battlefield.isAiVsAi) {
			hotkeysItem = new InGameMenuItem();
			hotkeysItem.title = "Hotkeys";
			hotkeysItem.inGameMenu = this;
			hotkeysItem.playerAction = inputs.toggleHotkeys;
			hotkeysItem.menuAction = HandleHotkeys;
			menu.AddItem(hotkeysItem.menuItem);
		}

		if (inputs.nextCamera.Bindings.Count > 0) {
			/*
			previousCameraItem = new InGameMenuItem();
			previousCameraItem.inGameMenu = this;
			previousCameraItem.title = "< Camera";
			previousCameraItem.menuAction = HandlePreviousCamera;
			menu.AddItem(previousCameraItem.menuItem);
			*/

			nextCameraItem = new InGameMenuItem();
			nextCameraItem.inGameMenu = this;
			nextCameraItem.playerAction = inputs.nextCamera;
			nextCameraItem.title = "Camera";
			nextCameraItem.menuAction = HandleCamera;
			menu.AddItem(nextCameraItem.menuItem);
		}

		menu.SetOrientation(MenuOrientation.Horizontal);
		menu.SetAnchor(menuPlacement);
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



	void HandleConcede() {
        GameObject listener = new GameObject();
        listener.name = "AnyKeyListener";
        AnyKeyListener anyKeyListener = listener.AddComponent<AnyKeyListener>();
        anyKeyListener.Listen(inputs, ReallyConcede, Reset, inputs.concede);
		menu.Destroy();

		menu = UI.Menu();
		menu.AddItem(UI.MenuItem("Confirm", ReallyConcede));
		menu.AddItem(UI.MenuItem("Cancel", Reset), true);
		menu.SetOrientation(MenuOrientation.Horizontal);
		menu.SetAnchor(menuPlacement);
		menu.backgroundColor = new Color(0, 0, 0, 0);
		menu.selectsOnShow = true;
		menu.inputs = inputs;
		menu.Show();
	}

	void ReallyConcede() {
		playingGameState.EndGame(player.opponent);
	}

	//TODO: different for each player?
	void HandleHotkeys() {
		App.shared.prefs.keyIconsVisible = !App.shared.prefs.keyIconsVisible;
        foreach (Tower _tower in GameObject.FindObjectsOfType<Tower>()) {
            _tower.UpdateHotKeys();
        }
	}

	void HandleCamera() {
		App.shared.cameraController.NextPosition();
	}

	//Matchmaker Menu

	public void ConnectMatchmakerMenu() {
		App.shared.Log("ConnectMatchmakerMenu: " + menu, this);
		App.shared.matchmaker.menu.nextMenu = menu;
		App.shared.matchmaker.menu.previousMenu = menu;
		App.shared.matchmaker.menu.orientation = MenuOrientation.Horizontal;

		if (menu != null) {
			menu.previousMenu = App.shared.matchmaker.menu;
			menu.nextMenu = App.shared.matchmaker.menu;
		}
	}

	public void DisconnectMatchmakerMenu() {
		App.shared.Log("DisconnectMatchmakerMenu: " + menu, this);
		App.shared.matchmaker.menu.nextMenu = null;
		App.shared.matchmaker.menu.previousMenu = null;
		App.shared.matchmaker.menu.orientation = MenuOrientation.Vertical;

		if (menu != null) {
			menu.previousMenu = null;
			menu.nextMenu = null;
		}
	}
}

public class InGameMenuItem {
	public InGameMenu inGameMenu;
	public string title;
	public InControl.PlayerAction playerAction;
	public System.Action menuAction;

	public UIButton menuItem {
		get {
			if (_menuItem == null) {
				_menuItem = UI.MenuItem(title, menuAction);
			}

			return _menuItem;
		}
	}

	public void ReadInput() {
		if (playerAction.WasPressed) {
			menuAction();
		}
	}

	UIButton _menuItem;
}

