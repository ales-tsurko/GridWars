using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : AppState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		RebuildMenu();
	}

	protected void RebuildMenu() {
		menu.Reset();
		ConfigureMenu();
		menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);
		menu.Show();
	}

	protected virtual void ConfigureMenu() {}

	protected virtual void Back() {}
}
