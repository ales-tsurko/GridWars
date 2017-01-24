using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartVet : TutorialPartDelegate {
	public override void DidBegin() {
		PauseBattlefield();

		tutorialPart.preventsNext = true;

		var tank = App.shared.battlefield.player1.units.Find(u => u is Tank);

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(GameUnit.GameUnitVeteranLevelChangedNotification)
			.SetAction(TankVeteranLevelChanged)
			.Add();

		foreach (var t in App.shared.battlefield.player2.fortress.towers) {
			var gameUnit = t.unitPrefab.GameUnit();

			if (gameUnit is MobileSAM) {
				for (var i = -1; i <= 1; i ++) {
					var unit = gameUnit.Instantiate();
					unit.player = App.shared.battlefield.player2;
					unit.transform.position = tank.transform.position + tank.transform.forward * 2 * tank.releaseZone.size.z + i * tank.transform.right * tank.releaseZone.size.x;
					unit.transform.rotation = tank.transform.rotation;
					unit.transform.Rotate(new Vector3(0, 180, 0));
					foreach (var weapon in unit.Weapons()) {
						weapon.isActive = false;
					}
				}
			}
		}
	}

	public override void TextDidComplete() {
		ResumeBattlefield();
	}

	void TankVeteranLevelChanged(Notification n) {
		App.shared.notificationCenter.RemoveObserver(this);
		tutorialPart.Next();
	}
}
