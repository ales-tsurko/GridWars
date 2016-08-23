using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class App : MonoBehaviour {

	public AssemblyCSharp.TimerCenter timerCenter;
	private static App _shared;

	public static App shared {
		get {
			if (_shared == null) {
				GameObject go = new GameObject();
				_shared = go.AddComponent<App>();
			}
			return _shared;
		}
	}

	public void Start() {
		timerCenter = new AssemblyCSharp.TimerCenter();
	}

	public void FixedUpdate() {
		timerCenter.Step();
	}

}
