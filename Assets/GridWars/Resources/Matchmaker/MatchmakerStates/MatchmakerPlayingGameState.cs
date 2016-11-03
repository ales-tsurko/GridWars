using UnityEngine;
using System.Collections;

public class MatchmakerPlayingGameState : MatchmakerState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Hide();
	}
}
