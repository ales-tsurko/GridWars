﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 *  This is a singleton. Access like this:
 * 
 *  App.shared.ResourcePathForUnitType(typeof(Tank))
 * 
 */

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

	// Game Loop -------------------

	public void Start() {
		timerCenter = new AssemblyCSharp.TimerCenter();
	}

	public void FixedUpdate() {
		timerCenter.Step();
	}

	// Finding Paths --------------------
	// should really have the Types themselve know how to find themselves but
	// class methods 

	public string ResourcePathForUnitType(System.Type type) {
		List <string> pathComponents = new List<string>();

		while (type != typeof(GameUnit)) {
			pathComponents.Add(type.Name);
			type = type.BaseType;
		}

		pathComponents.Add("GameUnit"); // add GameUnit
		pathComponents.Reverse();
		return string.Join("/", pathComponents.ToArray());
	}

	public string PrefabPathForUnitType(System.Type type) {
		string path = ResourcePathForUnitType(type);
		return path + "/Prefabs/" + type.Name;
	}

	public string SoundPathForUnitType(System.Type type) {
		string path = ResourcePathForUnitType(type);
		string soundPath = path + "/Sounds/";
		return soundPath;
	}

	public AudioClip SoundNamedForUnitType(string name, System.Type type) {
			string path = ResourcePathForUnitType(type);
			string soundPath = path + "/Sounds/birth";
			return Resources.Load<AudioClip>(soundPath);
	}

}
