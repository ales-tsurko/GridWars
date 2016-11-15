using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class AwaitRematchResponseState : PostGameSubState {
	Timer timer;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		timer = app.timerCenter.NewTimer();
		timer.action = ShowLeave;
		timer.timeout = 5;
		timer.Start();

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("WAITING FOR RESPONSE"));
		menu.Show();
	}

	public override void WillExit() {
		base.WillExit();

		timer.Cancel();
	}

	void ShowLeave() {
		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("WAITING FOR RESPONSE"));
		menu.AddItem(UI.MenuItem("Leave", postGameState.Leave), true);
		menu.Show();
	}
}
