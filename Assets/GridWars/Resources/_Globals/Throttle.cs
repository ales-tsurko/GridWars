using UnityEngine;
using System.Collections;

public class Throttle {
	public MonoBehaviour behaviour;
	public int period;

	public Throttle() {
		app = App.shared; //perf opt
	}

	public bool isOff {
		get {
			var bucket = (int)((uint)behaviour.GetHashCode() % (uint)period);
			//Debug.Log(App.shared.timeCounter + " " + period + " " + bucket);
			return (app.timeCounter % period == bucket);
		}
	}

	App app;
}
