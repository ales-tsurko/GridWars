using UnityEngine;
using System.Collections.Generic;

public class InGameMenu {
	public PlayingGameState playingGameState;
	public Player player;

	public int localPlayerNumber {
		get {
			if (isLocalPlayer1) {
				return 1;
			}
			else {
				return 2;
			}
		}
	}

	public bool isAiVsAi {
		get {
			return player == null;
		}
	}

	public bool isFocused {
		get {
			return new List<UIButton>(menu.GetComponentsInChildren<UIButton>()).Find(b => b.isSelected) != null;
		}
	}

	public void Show() {
		App.shared.ResetMenu();

		Reset();
	}

	public void Hide() {
		if (menu != null) {
			menu.Destroy();
		}
	}

	public void ReadInput () {
		concedeItem.ReadInput();

		if (hotkeysItem != null) {
			hotkeysItem.ReadInput();
		}

		if (cameraItem != null) {
			cameraItem.ReadInput();
		}

		if ((Keys.FOCUSMENU + localPlayerNumber).KeyDown() && !App.shared.cameraController.isInFirstPerson) {
			if (isFocused) {
				menu.LoseFocus();
			}
			else {
				menu.SelectNextItem();
			}
		}
	}

	UIMenu menu;
	InGameMenuItem concedeItem;
	InGameMenuItem hotkeysItem;
	InGameMenuItem cameraItem;

	bool isLocalPlayer1 {
		get {
			return isAiVsAi || player.localNumber == 1;
		}
	}

	MenuAnchor menuAnchor {
		get {
			if (player == null || player.localNumber == 1) {
				return MenuAnchor.TopLeft;
			}
			else {
				return MenuAnchor.TopRight;
			}
		}
	}

	void Reset() {
		if (menu != null) {
			menu.Destroy();
		}

		menu = UI.Menu();

		string title;

		//TODO: detect which player conceded in shared screen pvp
		if (player == null) { //AIvAI
			title = "Quit";
		}
		else {
			title = "Concede";
		}

		concedeItem = new InGameMenuItem();
		concedeItem.inGameMenu = this;
		concedeItem.title = title;
		concedeItem.keyName = Keys.CONCEDE;
		concedeItem.action = HandleConcede;
		menu.AddItem(concedeItem.menuItem);

		if (!isAiVsAi) {
			hotkeysItem = new InGameMenuItem();
			hotkeysItem.inGameMenu = this;
			hotkeysItem.title = "Hotkeys";
			hotkeysItem.keyName = Keys.TOGGLEKEYS;
			hotkeysItem.action = HandleHotkeys;
			menu.AddItem(hotkeysItem.menuItem);
		}

		if (isLocalPlayer1) {
			cameraItem = new InGameMenuItem();
			cameraItem.inGameMenu = this;
			cameraItem.title = "Camera";
			cameraItem.keyName = Keys.CHANGECAM;
			cameraItem.action = HandleCamera;
			menu.AddItem(cameraItem.menuItem);
		}

		menu.controllerInputName = isLocalPlayer1 ? UIMenu.CONTROLLER_1_MENU_CURSOR_NAME : UIMenu.CONTROLLER_2_MENU_CURSOR_NAME;
		menu.controllerSelectionKey = isLocalPlayer1 ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1;

		menu.SetOrientation(MenuOrientation.Horizontal);
		menu.SetAnchor(menuAnchor);
		menu.SetBackground(Color.black, 0);
		menu.selectsOnShow = false;
		menu.Show();
	}


	void HandleConcede() {
		menu.Destroy();

		menu = UI.Menu();
		menu.AddItem(UI.MenuItem("Confirm", ReallyConcede));
		menu.AddItem(UI.MenuItem("Cancel", Reset));
		menu.SetOrientation(MenuOrientation.Horizontal);
		menu.SetAnchor(menuAnchor);
		menu.SetBackground(Color.black, 0);
		menu.selectsOnShow = true;
		menu.controllerInputName = isLocalPlayer1 ? UIMenu.CONTROLLER_1_MENU_CURSOR_NAME : UIMenu.CONTROLLER_2_MENU_CURSOR_NAME;
		menu.controllerSelectionKey = isLocalPlayer1 ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1;
		menu.Show();
	}

	void ReallyConcede() {
		var state = new PostGameState();

		if (isAiVsAi) {
			state.victoriousPlayer = null;
		}
		else {
			state.victoriousPlayer = player.opponent;
		}
			
		if (App.shared.battlefield.isInternetPVP) {
			App.shared.Log("ConcedeEvent.Send", this);
			ConcedeEvent.Create(Bolt.GlobalTargets.Others, Bolt.ReliabilityModes.ReliableOrdered).Send();
		}

		playingGameState.TransitionTo(state);
	}

	//TODO: different for each player?
	void HandleHotkeys() {
		App.shared.prefs.keyIconsVisible = !App.shared.prefs.keyIconsVisible;
	}

	void HandleCamera() {
		App.shared.cameraController.NextPosition();
	}
}

public class InGameMenuItem {
	public InGameMenu inGameMenu;
	public string title;
	public string keyName;
	public System.Action action;

	public UIButton menuItem {
		get {
			if (_menuItem == null) {
				_menuItem = UI.MenuItem(title + " (" + keyMappingName.GetKeyCode().ToString() + ")", action);
			}

			return _menuItem;
		}
	}

	public void ReadInput() {
		if (keyMappingName.KeyDown()) {
			action();
		}
	}

	UIButton _menuItem;

	string keyMappingName {
		get {
			return keyName + inGameMenu.localPlayerNumber;
		}
	}
}