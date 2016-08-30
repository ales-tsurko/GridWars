using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

	public float destroyTime = 10f;

	AssemblyCSharp.Timer timer;

	void Start () {
		timer = App.shared.timerCenter.NewTimer();
		timer.action = AddToDestroyQueue;
		timer.SetTimeout(destroyTime);
		timer.Start();
	}

	void AddToDestroyQueue() {
		App.shared.AddToDestroyQueue(gameObject);
	}

	void OnDestroy() {
		timer.Cancel();
	}
}
