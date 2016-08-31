using UnityEngine;
using System.Collections;

	public class Wreckage : MonoBehaviour {

	private float sinkPeriod;
	private float deathHeight;
	private AssemblyCSharp.Timer timer;
	private float miny = 0;

	public void Start () {
		sinkPeriod = 6f; 
		VerifyLayer();
		BoxCollider bc = gameObject.GetComponent<BoxCollider>();
		deathHeight = bc.bounds.size.y * 2f;
	}

	public void VerifyLayer() {
		if (gameObject.layer != LayerMask.NameToLayer("Wreckage")) {
			print("WARNING: you forgot to set the layer of this object to Wreckage");
			gameObject.layer = LayerMask.NameToLayer("Wreckage");
		}
	}

	public void StartRemoveTimer() {
		timer = App.shared.timerCenter.NewTimer();
		timer.action = AddToDestroyQueue;
		timer.SetTimeout(sinkPeriod);
		timer.Start();
	}

	public void DisableCollisions() {
		gameObject.GetComponent<Rigidbody>().detectCollisions = false;
		gameObject.GetComponent<Collider>().enabled = false;

	}

	public void UpdateMinY() {
		if (timer.ratioDone() > .5) {
			miny = - (timer.ratioDone() - 0.5f) * 2f * deathHeight;
		}
	}
	 
	public void FixedUpdate() {

		if (timer == null) {
			if (IsStill()) {
				StartSinking();
			}
		} else {
			UpdateMinY();
			Vector3 pos = transform.position;
			pos.y = miny;
			transform.position = pos;
		}
	}

	public bool IsStill() {
		return Mathf.Approximately(gameObject.GetComponent<Rigidbody>().velocity.sqrMagnitude, 0.0f);
	}

	public void StartSinking() {
		DisableCollisions();
		StartRemoveTimer();
	}

	void AddToDestroyQueue() {
		App.shared.AddToDestroyQueue(gameObject);
	}

	void OnDestroy() {
		timer.Cancel();
	}
}
