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

	private static App _shared;
	public AssemblyCSharp.TimerCenter timerCenter;
	public int timeCounter = 0;
	public AssemblyCSharp.StepCache stepCache;
	private List <GameObject> _destroyQueue;

	public static App shared {
		get {
			if (_shared == null) {
				GameObject go = new GameObject();
				_shared = go.AddComponent<App>();
			}
			return _shared;
		}
	}

	// --- Game Loop -------------------

	public void Start() {
		timerCenter = new AssemblyCSharp.TimerCenter();
		stepCache = new AssemblyCSharp.StepCache();
		_destroyQueue = new List<GameObject>();

		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;
	}

	public void FixedUpdate() {
		timerCenter.Step();
		stepCache.Step();
		timeCounter++;
	}

	// --- Finding Paths --------------------
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
			string soundPath = path + "/Sounds/" + name;
			return Resources.Load<AudioClip>(soundPath);
	}

	// --- Destroying Objects -----------

	void LateUpdate() {
		ProcessDestroyQueue();
	}

	public void AddToDestroyQueue(GameObject obj) {
		if(!_destroyQueue.Contains(obj)) {
			_destroyQueue.Add(obj);
		}
	}

	public void ProcessDestroyQueue() {
		foreach(GameObject obj in _destroyQueue) {
			//Debug.Log("ProcessDestroyQueue: " + obj);
			GameUnit gameUnit;
			if ((gameUnit = obj.GetComponent<GameUnit>()) != null) {
				gameUnit.ActuallyDestroySelf();
			}
			else {
				Destroy(obj);
			}
		}
		_destroyQueue.Clear();
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