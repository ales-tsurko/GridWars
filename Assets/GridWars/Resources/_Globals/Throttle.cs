using UnityEngine;
using System.Collections;

public class Throttle {
	public MonoBehaviour behaviour;
	public int _period;
	public int _bucket;
		
	public Throttle() {
		app = App.shared; //perf opt
	}

	public int period {
		set {
			_period = value;
			_bucket = (int)((uint)behaviour.GetHashCode() % (uint)period);
		}
		get {
			return _period;
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
