using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

	public float destroyTime = 10f;

	AssemblyCSharp.Timer timer;
	ParticleSystem ps;
	void Start () {
		ps = GetComponent<ParticleSystem> ();
		if (ps != null) {
			return;
		}
		timer = App.shared.timerCenter.NewTimer();
		timer.action = DestroyObject;
		timer.SetTimeout(destroyTime);
		timer.Start();
	}

	void DestroyObject() {
		var gameUnit = GetComponent<GameUnit>();
		if (gameUnit == null) {
			Destroy(gameObject);
		}
		else if (BoltNetwork.isServer){
			gameUnit.Die();
		}
		//App.shared.AddToDestroyQueue(gameObject);
	}

	void Update (){
		if (ps == null || Time.frameCount % 60 != 0) {
			return;
		}
		if (!ps.IsAlive (true)) {
			Destroy(gameObject);
		}

	}
	void OnDestroy() {
		if (timer != null) { //in case start is never called.
			timer.Cancel();
		}
	}
}
