using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartPower : TutorialPartDelegate {
	public override void DidBegin() {
		App.shared.battlefield.player1.powerSource.power = 0f;
		ResumeBattlefield();

	}
}
