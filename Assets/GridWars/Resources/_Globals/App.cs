using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

/*
 *  This is a singleton. Access like this:
 * 
 *  App.shared.ResourcePathForUnitType(typeof(Tank))
 * 
 */

public class App : MonoBehaviour {

	public AssemblyCSharp.TimerCenter timerCenter;
	private static App _shared;
	public int timeCounter = 0;

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
		//BTest.ClassTest();

		timerCenter = new AssemblyCSharp.TimerCenter();
		Application.targetFrameRate = 60;

	}

	public void FixedUpdate() {
		timerCenter.Step();
		timeCounter++;
	}

	// Finding Paths --------------------
	// should really have the Types themselve know how to find themselves but
	// class methods don't have access to the type on which they were called -
	// they only know the type in which they are declared

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


/*
public class ATest : MonoBehaviour {
	public static void ClassTest() {
		print("type = " + MethodBase.GetCurrentMethod().ReflectedType.GetType().Name);
	}
}

public class BTest : ATest {
}
*/