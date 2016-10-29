using UnityEngine;
using System.Collections;

public class FriendsListState : AppState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.ResetMenu();

		if (app.account.friends.Count == 0) {
			var item = UI.MenuItem("Share this code with friends to play against them:");
			item.isInteractible = false;
			item.matchesNeighborSize = false;
			app.menu.AddItem(item);

			item = UI.MenuItem("AE14S");
			item.isInteractible = false;
			item.matchesNeighborSize = false;
			app.menu.AddItem(item);

			item = UI.MenuItem("Back", Back);
			item.isBackItem = true;
			app.menu.AddItem(item);

			app.menu.Show();
		}
		else {
			
		}
	}

	void Back() {
		TransitionTo(new MainMenuState());
	}
}
