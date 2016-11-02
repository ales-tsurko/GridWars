using UnityEngine;
using System.Collections;

public class AppState : MatchmakerMenuDelegate {
	public AppStateOwner owner;

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

		matchmaker.menu.AddDelegate(this);
		ConfigureMatchmakerMenu();
	}

	public virtual void WillExit() {
		matchmaker.menu.RemoveDelegate(this);
	}

	public virtual void Update() {}

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

	public virtual void ConfigureMatchmakerMenu() {
		if (matchmaker.menu.isOpen) {
			DisconnectMatchmakerMenu();
		}
		else {
			ConnectMatchmakerMenu();
		}
	}

	public virtual void MatchmakerMenuOpened() {
		this.menu.Hide();
		DisconnectMatchmakerMenu();
	}

	public virtual void MatchmakerMenuClosed() {
		var selectsOnShow = this.menu.selectsOnShow;
		this.menu.selectsOnShow = false;
		this.menu.Show();
		this.menu.selectsOnShow = selectsOnShow;
		ConnectMatchmakerMenu();
	}

	public virtual void ConnectMatchmakerMenu() {
		matchmaker.menu.nextMenu = menu;
		matchmaker.menu.previousMenu = menu;
		matchmaker.menu.orientation = MenuOrientation.Vertical;

		if (menu != null) {
			menu.previousMenu = matchmaker.menu;
			menu.nextMenu = matchmaker.menu;
		}
	}

	public virtual void DisconnectMatchmakerMenu() {
		matchmaker.menu.nextMenu = null;
		matchmaker.menu.previousMenu = null;
		matchmaker.menu.orientation = MenuOrientation.Vertical;

		if (menu != null) {
			menu.previousMenu = null;
			menu.nextMenu = null;
		}
	}
}

public interface AppStateOwner {
	AppState state { get; set; }
}