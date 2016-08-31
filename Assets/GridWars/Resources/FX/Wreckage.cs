using UnityEngine;
using System.Collections;

	/*
	 * Setting this script on an object will
	 * disable it's collisions with the Default layer,
	 * wait until the object comes to rest, then 
	 * cause it to sink into the ground and then be removed from the game.
	 * 
	 * */

	public class Wreckage : MonoBehaviour {

	private float sinkPeriod  = 6f;
	private float chillPeriod = 6f;
	private float deathHeight;
	private AssemblyCSharp.Timer chillTimer = null;
	private AssemblyCSharp.Timer sinkTimer  = null;

	public void Start () {
		VerifyLayer();
		Collider bc = gameObject.GetComponent<Collider>();
		deathHeight = bc.bounds.size.y * 2f;
	}

	public void VerifyLayer() {
		if (gameObject.layer != LayerMask.NameToLayer("Wreckage")) {
			print("WARNING: you forgot to set the layer of this object to Wreckage");
			gameObject.layer = LayerMask.NameToLayer("Wreckage");
		}
	}

	public void FixedUpdate() {
		if (chillTimer == null) {
			if (IsStill()) {
				StartChillTimer();
			}
		} else if (sinkTimer != null) {
			SinkStep();
		}
	}

	public void StartChillTimer() {

		chillTimer = App.shared.timerCenter.NewTimer();
		chillTimer.action = StartSinkTimer;
		chillTimer.SetTimeout(chillPeriod);
		chillTimer.Start();

	}

	public void StartSinkTimer() {
		sinkTimer = App.shared.timerCenter.NewTimer();
		sinkTimer.action = AddToDestroyQueue;
		sinkTimer.SetTimeout(sinkPeriod);
		sinkTimer.Start();

		DisableRemainingCollisions();
	}

	public void DisableRemainingCollisions() {
		/*
		gameObject.GetComponent<Rigidbody>().detectCollisions = false;
		gameObject.GetComponent<Collider>().enabled = false;
		*/
	}

	public void SinkStep() {
		Vector3 pos = transform.position;
		pos.y = - sinkTimer.ratioDone() * deathHeight;
		transform.position = pos;
	}

	public bool IsStill() {
		return Mathf.Approximately(gameObject.GetComponent<Rigidbody>().velocity.sqrMagnitude, 0.0f);
	}
		
	void AddToDestroyQueue() {
		App.shared.AddToDestroyQueue(gameObject);
	}

	void OnDestroy() {
		if (chillTimer != null) {
			chillTimer.Cancel();
		}
		if (sinkTimer != null) {
			sinkTimer.Cancel();
		}
	}
}
