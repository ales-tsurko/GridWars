using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartBuildTank : TutorialPartDelegate {
	public override void DidBegin() {
		tutorialPart.preventsNext = true;

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(Tower.TowerProducedUnitNotification)
			.SetAction(TowerProducedUnit)
			.Add();

		//Debug.Log("GameUnit.Load<Tank>().PowerCost(0): " + GameUnit.Load<Tank>().PowerCost(0));
		App.shared.battlefield.player1.powerSource.power = GameUnit.Load<Tank>().PowerCost(0);
		foreach(var tower in App.shared.battlefield.player1.fortress.towers) {
			tower.isDisabled = !(tower.unitPrefab.GameUnit() is Tank);
		}

		App.shared.battlefield.player1.npcModeOn = false;

		ResumeBattlefield();
	}

	void TowerProducedUnit(Notification n) {
		App.shared.notificationCenter.RemoveObserver(this);

		App.shared.battlefield.player1.npcModeOn = true;

		(n.data as Tank).gameObject.name = "Tutorial Tank";


		StartCoroutine(WaitThenNext());
	}

	IEnumerator WaitThenNext() {
		yield return new WaitForSeconds(0.5f); //otherwise the Tank won't appear in player1.units
		tutorialPart.Next();
	}
}
