using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartUnits : TutorialPartDelegate {
	public override void DidBegin() {
		App.shared.battlefield.player1.powerSource.power = 0f;
	}
}
