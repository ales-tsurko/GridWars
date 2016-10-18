using UnityEngine;
using System.Collections;

public class Throttle {
	public MonoBehaviour _behaviour;
	public int _period;
	public int _bucket;
		
	public Throttle() {
		app = App.shared; //perf opt
	}

	public int period {
		set {
			_period = value;
			UpdateBucket();
		}
		get {
			return _period;
		}
	}

	public MonoBehaviour behaviour {
		set {
			_behaviour = value;
			UpdateBucket();
		}
		get {
			return _behaviour;
		}
	}

	void UpdateBucket() {
		if (behaviour != null && period != 0) {
			_bucket = (int)((uint)behaviour.GetHashCode() % (uint)period);
		}
	}

	public bool isOff {
		get {
			//var bucket = (int)((uint)behaviour.GetHashCode() % (uint)period);
			//Debug.Log(App.shared.timeCounter + " " + period + " " + bucket);
			return (app.timeCounter % period == _bucket);
		}
	}

	App app;
}
