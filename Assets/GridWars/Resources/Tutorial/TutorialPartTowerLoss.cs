using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartTowerLoss : TutorialPartDelegate {
	public override void DidBegin() {
		PauseBattlefield();

		tutorialPart.preventsNext = true;

		App.shared.battlefield.player1.powerSource.power = 0f;

		var tank = App.shared.battlefield.player1.units.Find(u => u is Tank);

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(Tower.TowerDiedNotification)
			.SetAction(TowerDied)
			.Add();

		foreach (var t in App.shared.battlefield.player2.fortress.towers) {
			var gameUnit = t.unitPrefab.GameUnit();

			if (gameUnit is Tank) {
				var unit = gameUnit.Instantiate();
				unit.player = App.shared.battlefield.player2;
				unit.transform.position = tank.transform.position + tank.transform.forward * 2 * tank.releaseZone.size.z;
				unit.transform.rotation = tank.transform.rotation;
				unit.transform.Rotate(new Vector3(0, 180, 0));
			}
		}

		tank.Die();
	}

	public override void TextDidComplete() {
		ResumeBattlefield();
	}

	void TowerDied(Notification n) {
		App.shared.notificationCenter.RemoveObserver(this);
		StartCoroutine(WaitThenAllowNext());
	}

	IEnumerator WaitThenAllowNext() {
		yield return new WaitForSeconds(2.0f); //Give player a chance to see tower die
		PauseBattlefield();
		tutorialPart.Next();
	}
}
