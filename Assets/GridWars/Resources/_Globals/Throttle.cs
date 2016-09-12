using UnityEngine;
using System.Collections;

public class Throttle {
	public MonoBehaviour behaviour;
	public int period;

	public bool isOff {
		get {
			var bucket = (int)((uint)behaviour.GetHashCode() % (uint)period);
			return (App.shared.timeCounter % period == bucket);
		}
	}
}
