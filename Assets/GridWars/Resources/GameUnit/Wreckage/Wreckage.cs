using UnityEngine;
using System.Collections;

public class Wreckage : GameUnit {

	public float destroyPeriod;
	private float deathHeight;
	private AssemblyCSharp.Timer timer;


	public override void ServerAndClientJoinedGame() {
		// avoid normal setup
	}

	public override void ServerJoinedGame () {
		timer = App.shared.timerCenter.NewTimer();
		timer.action = AddToDestroyQueue;
		timer.SetTimeout(destroyPeriod);
		timer.Start();

		BoxCollider bc = gameObject.GetComponent<BoxCollider>();
		deathHeight = bc.bounds.size.y * 2f;

		// ignore collisions with ground
		GameObject bf = GameObject.Find("BattlefieldPlane");
		Collider mc = bf.GetComponent<Collider>();

		Physics.IgnoreCollision(bc, mc, true);
	}

	public override void ServerFixedUpdate() {
		Vector3 pos = transform.position;
		if (timer.ratioDone() < .5) {
			pos.y = 0;
		} else {
			pos.y = - (timer.ratioDone() - 0.5f) * 2f * deathHeight;
		}
		transform.position = pos;
	}

	void AddToDestroyQueue() {
		App.shared.AddToDestroyQueue(gameObject);
	}

	void OnDestroy() {
		timer.Cancel();
	}

	public override void ApplyDamage(float v) {

	}
}
