using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartBase : TutorialPartDelegate {
	public override void DidBegin() {
		App.shared.battlefield.player1.powerSource.MakeMax();
	}
}
