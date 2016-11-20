using UnityEngine;
using System.Collections;

public class AppState {
	public AppStateOwner owner;

	public string openSoundName = null;
	public string openSoundtrackName = null;
	public bool connectsMatchmakerMenu = false;
	public bool interactibleMatchmakerMenu = false;

	public static string AppStateChangedNotification = "AppStateChangedNotification";

	// --- sounds ---

	void HandleOpenSounds() {
		if (openSoundtrackName != null) {
			App.shared.SoundtrackNamed(openSoundtrackName).Play();
		}

		if (openSoundName == null) {
			App.shared.PlayAppSoundNamedAtVolume(openSoundName, 1f);
		}
	}

	void HandleCloseSounds() {
		if (openSoundtrackName != null) {
			App.shared.SoundtrackNamed(openSoundtrackName).FadeOut();
		}
	}

	// -----------------

	public string name {
		get {
			return this.GetType().ToString();
		}
	}

	public void TransitionTo(AppState state) {
		owner.state = state;
		state.owner = owner;
		this.WillExit();
		state.EnterFrom(this);
		app.notificationCenter.NewNotification()
			.SetName(AppStateChangedNotification)
			.SetSender(state)
			.Post();
	}

	public virtual void EnterFrom(AppState state) {
		if (state != null) {
			app.Log("EnterFrom: " + state.name, this);
		}
		else {
			app.Log("EnterFrom null", this);
		}

		HandleOpenSounds();

		/*
		if (connectsMatchmakerMenu) {
			ConnectMatchmakerMenu();
		}
		else {
			DisconnectMatchmakerMenu();
		}
		matchmaker.menu.isInteractible = interactibleMatchmakerMenu;
		*/
	}

	public virtual void WillExit() {
		HandleCloseSounds();
	}

	public virtual void Update() {
	}

	protected App app {
		get {
			return App.shared;
		}
	}

	UIMenu _menu;
	public UIMenu menu {
		get {
			if (_menu == null) {
				return App.shared.menu;
			}
			else {
				return _menu;
			}
		}

		set {
			_menu = value;
		}
	}

	protected Battlefield battlefield {
		get {
			return App.shared.battlefield;
		}
	}

	protected Network network {
		get {
			return App.shared.network;
		}
	}

	Matchmaker _matchmaker;
	public Matchmaker matchmaker {
		get {
			if (_matchmaker == null) {
				return App.shared.matchmaker;
			}
			else {
				return _matchmaker;
			}
		}

		set {
			_matchmaker = value;
		}
	}

	public virtual void ConnectMatchmakerMenu() {
		//app.Log("ConnectMatchmakerMenu", this);
		//app.Log("matchmaker.menu: " + matchmaker.menu, this);
		menu.nextMenu = matchmaker.menu;
		menu.previousMenu = matchmaker.menu;

		matchmaker.menu.nextMenu = menu;
		matchmaker.menu.previousMenu = menu;
	}

	public virtual void DisconnectMatchmakerMenu() {
		//app.Log("DisconnectMatchmakerMenu", this);
		menu.nextMenu = null;
		menu.previousMenu = null;

		matchmaker.menu.nextMenu = null;
		matchmaker.menu.previousMenu = null;
	}
}

public interface AppStateOwner {
	AppState state { get; set; }
}