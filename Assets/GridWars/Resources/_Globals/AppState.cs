using UnityEngine;
using System.Collections;

public class AppState {
	public string name {
		get {
			return this.GetType().ToString();
		}
	}

	public void TransitionTo(AppState state) {
		App.shared.state = state;
		state.EnterFrom(this);
	}

	public virtual void EnterFrom(AppState state) {
		if (state != null) {
			Debug.Log("AppState: " + state.name + " > " + this.name);
		}
	}

	public virtual void Update() {}

	protected App app {
		get {
			return App.shared;
		}
	}

	protected UIMenu menu {
		get {
			return App.shared.menu;
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

	protected Matchmaker matchmaker {
		get {
			return App.shared.matchmaker;
		}
	}
}
