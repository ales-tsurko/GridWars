using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartSam : TutorialPartDelegate {
	public override void DidBegin() {
		App.shared.battlefield.player1.powerSource.power = GameUnit.Load<MobileSAM>().PowerCost(0);
		foreach(var tower in App.shared.battlefield.player1.fortress.towers) {
			tower.isDisabled = !(tower.unitPrefab.GameUnit() is MobileSAM);
		}
		PauseBattlefield();
	}
}
