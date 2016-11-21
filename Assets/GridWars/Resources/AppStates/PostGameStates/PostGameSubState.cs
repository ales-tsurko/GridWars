using UnityEngine;
using System.Collections;

public class PostGameSubState : AppState {
	public PostGameState postGameState {
		get {
			return owner as PostGameState;
		}
	}

	public override UIMenu[] focusableMenus {
		get {
			return new UIMenu[]{};
		}
	}

	public virtual void ReceivedRematchRequest() {
	}

	public virtual void ReceivedAcceptRematch() {
	}

	public virtual void GameCancelled() {
		ShowOpponentLeft();
	}

	void ShowOpponentLeft() {
		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("Opponent Left"));
		menu.AddItem(UI.MenuItem("Leave", postGameState.Leave));
		menu.Show();
	}
}
