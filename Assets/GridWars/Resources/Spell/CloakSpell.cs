using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakSpell : Spell {

	// cloak unit (making it invisible to enemies) until it fires
	// while cloaked, unit renders as semi-transparent

	override public float Cost() {
		return 20f;
	}

	override public float LifeSpan() {
		if (gameUnit.GetType() == typeof(Tanker)) {
			return 3.7f;
		}
		return 30f;
	}

	public float cloackedAlpha = 0.3f;

	override public void ServerAndClientInit () {
		base.ServerAndClientInit();

		gameUnit.SetIsCloaked(true);

		gameUnit.gameObject.SetAlpha(cloackedAlpha);

		// so we can uncloak when we fire
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
