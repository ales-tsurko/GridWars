using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

	public float destroyTime = 10f;

	AssemblyCSharp.Timer timer;

	void Start () {
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
		else {
			gameUnit.Die();
		}
		//App.shared.AddToDestroyQueue(gameObject);
	}

	void OnDestroy() {
		if (timer != null) { //in case start is never called.
			timer.Cancel();
		}
	}
}
