using UnityEngine;
using System.Collections;

public class DeathMelt : MonoBehaviour {

	public float destroyPeriod;
	private float height;

	AssemblyCSharp.Timer timer;

	void Start () {
		timer = App.shared.timerCenter.NewTimer();
		timer.action = AddToDestroyQueue;
		timer.SetTimeout(destroyPeriod);
		timer.Start();

		// TODO: calc height
		BoxCollider bc = gameObject.GetComponent<BoxCollider>();
		height = bc.bounds.size.y;

		// ignore collisions with ground
		GameObject bf = GameObject.Find("BattlefieldPlane");
		Collider mc = bf.GetComponent<Collider>();

		//GameObject bf = Battlefield.current();
		Physics.IgnoreCollision(bc, mc, true);
	}


	public void FixedUpdate() {
		Vector3 pos = transform.position;
		pos.y = - timer.ratioDone() * height / 10f;
		transform.position = pos;
	}

	void AddToDestroyQueue() {
		App.shared.AddToDestroyQueue(gameObject);
	}

	void OnDestroy() {
		timer.Cancel();
	}
}
