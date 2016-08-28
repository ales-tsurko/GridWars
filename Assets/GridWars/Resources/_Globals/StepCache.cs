using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp {

	public class StepCache {
		
		Dictionary<string, GameObject[]> typeCache;
		List<GameObject> _activeGameObjects;
		List<GameUnit> _allVehicles;

		public StepCache() {
			typeCache = new Dictionary<string, GameObject[]>(); 
		}

		public void Step() {
			typeCache.Clear(); 
			_activeGameObjects = null;
			if (_allVehicles != null) {
				_allVehicles = null;
			}
		}

		/*
		public GameObject[] CacheFindObjectsOfType(System.Type aType) {
			string key = aType.Name;

			if (!typeCache.ContainsKey(key)) {
				GameObject[] objs = (GameObject[])UnityEngine.Object.FindObjectsOfType(aType);
				typeCache.Add(key, objs);
			}

			return typeCache[key];
		}
		*/

		public List<GameObject> ActiveGameObjects() {
			if (_activeGameObjects == null) {
				GameObject[] objs = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
				_activeGameObjects = new List<GameObject>();
				foreach (GameObject obj in objs) {
					if (obj.activeInHierarchy) {
						if (!obj.IsDestroyed()) {
							_activeGameObjects.Add(obj);
						}
					}
				}
			}

			for (int i = _activeGameObjects.Count - 1; i >= 0; i--) {
				if (_activeGameObjects[i].IsDestroyed()) { 
					_activeGameObjects.RemoveAt(i);
				}
			}
			return _activeGameObjects;
		}

		public List<GameUnit> AllVehicles() {
			if (_allVehicles == null) {
				List <GameObject> objs = ActiveGameObjects();
				_allVehicles = new List<GameUnit>();

				foreach (GameObject obj in objs) {
					GameUnit unit = obj.GameUnit();
					if (unit && unit.IsOfType(typeof(Vehicle))) {
						_allVehicles.Add(unit);
					}
				}
			}

			for (int i = _allVehicles.Count - 1; i >= 0; i--) {
				if (_allVehicles[i].gameObject.IsDestroyed()) { 
					_allVehicles.RemoveAt(i);
				}
			}
			return _allVehicles;
		}

	}
}
