using UnityEngine;
using System.Collections;

public class AppState {
	public AppStateOwner owner;

	public string openSoundName = null;
	public string openSoundtrackName = null;

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
	}

	public virtual void EnterFrom(AppState state) {
		if (state != null) {
			app.Log("EnterFrom: " + state.name, this);
		}
		else {
			app.Log("EnterFrom null", this);
		}

		HandleOpenSounds();
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
}

public interface AppStateOwner {
	AppState state { get; set; }
}