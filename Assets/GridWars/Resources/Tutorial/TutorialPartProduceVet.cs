using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartProduceVet : TutorialPartDelegate {
	public override void DidBegin() {
		StartCoroutine(WaitThenBegin());
	}

	IEnumerator WaitThenBegin() {
		yield return new WaitForSeconds(2f); //Let tank run for a few seconds

		var tank = App.shared.battlefield.player1.units.Find(u => u is Tank);

		tank.Die();

		tutorialPart.preventsNext = true;

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(Tower.TowerProducedUnitNotification)
			.SetAction(TowerProducedUnit)
			.Add();

		App.shared.battlefield.player1.powerSource.power = GameUnit.Load<Tank>().PowerCost(1);
		foreach(var tower in App.shared.battlefield.player1.fortress.towers) {
			tower.isDisabled = !(tower.unitPrefab.GameUnit() is Tank);
		}

		App.shared.battlefield.player1.npcModeOn = false;

		ResumeBattlefield();
	}

	void TowerProducedUnit(Notification n) {
		App.shared.notificationCenter.RemoveObserver(this);

		var tank = n.data as Tank;

		if (tank.veteranLevel == 0) {
			tank.Die();
			App.shared.battlefield.player1.powerSource.power = GameUnit.Load<Tank>().PowerCost(1);
			tutorialPart.hoverText = "You released too fast.NEWLINENEWLINE.Press and hold longer to try again.";
			tutorialPart.Reset();
		}
		else {
			App.shared.battlefield.player1.npcModeOn = true;
			StartCoroutine(WaitThenNext());
		}
	}

	IEnumerator WaitThenNext() {
		yield return new WaitForSeconds(2f); //give player a chance to see the tank
		tutorialPart.Next();
	}
}
