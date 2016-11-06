using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class AwaitRematchResponseState : PostGameSubState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("WAITING FOR RESPONSE"));
		menu.AddItem(UI.MenuItem("Leave", postGameState.Leave), true);
		menu.Show();
	}
}
