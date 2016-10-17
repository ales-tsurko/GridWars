using UnityEngine;
using System.Collections;

public class InGameMenu {
	public PlayingGameState playingGameState;
	public Player player;

	public bool isAiVsAi {
		get {
			return player == null;
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

	}

	UIMenu menu;
	InGameMenuItem concedeItem;
	InGameMenuItem hotkeysItem;
	InGameMenuItem cameraItem;

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

		if (isAiVsAi || player.localNumber == 1) {
			cameraItem = new InGameMenuItem();
			cameraItem.inGameMenu = this;
			cameraItem.title = "Camera";
			cameraItem.keyName = Keys.CHANGECAM;
			cameraItem.action = HandleCamera;
			menu.AddItem(cameraItem.menuItem);
		}


		menu.SetOrientation(MenuOrientation.Horizontal);
		menu.SetAnchor(menuAnchor);
		menu.SetBackground(Color.black, 0);
		menu.isNavigable = false;
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
				Debug.Log(keyMappingName);
				_menuItem = UI.MenuItem(title + " (" + keyMappingName.GetKeyCode().ToString() + ")", action);
			}

			return _menuItem;
		}
	}

	public void ReadInput() {
		if (keyMappingName.KeyDown()) {
			Debug.Log(keyMappingName);
			action();
		}
	}

	UIButton _menuItem;

	string keyMappingName {
		get {
			return keyName + (inGameMenu.isAiVsAi ? "1" : inGameMenu.player.localNumber.ToString());
		}
	}
}