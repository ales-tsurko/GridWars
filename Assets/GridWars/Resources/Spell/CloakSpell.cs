using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakSpell : Spell {

	override public float Cost() {
		return 15f;
	}

	override public float LifeSpan() {
		return 1000f;
	}

	public float cloackedAlpha = 0.3f;

	override public void ServerAndClientInit () {
		base.ServerAndClientInit();

		gameUnit.SetIsCloaked(true);

		gameUnit.gameObject.SetAlpha(cloackedAlpha);



		App.shared.notificationCenter.NewObservation()
			.SetNotificationName("UnitDidFireNotification")
			.SetAction(UnitDidFireNotification)
			.SetSender(gameUnit)
			.Add();

	}

	override public void ServerAndClientFixedUpdate () {
		base.ServerAndClientFixedUpdate();

		float dt = TimeLeft();
		if (dt < 1f) {
			float a = Mathf.Clamp(cloackedAlpha + 1f - dt, 0f, 1f);
			gameUnit.gameObject.SetAlpha(a);

		} else {
			gameUnit.gameObject.SetAlpha(cloackedAlpha);
		}
	}

	override public void ServerAndClientStop () {
		base.ServerAndClientStop();

		gameUnit.SetIsCloaked(false);
		gameUnit.gameObject.SetAlpha(1f);
		App.shared.notificationCenter.RemoveObserver(this);
	}

	public void UnitDidFireNotification(Notification note) {
		ServerAndClientStop();
	}
}
