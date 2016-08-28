using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp {

	public class StepCache {
		
		Dictionary<string, GameObject[]> typeCache;
		List<GameObject> _activeGameObjects;
		List<GameObject> _allVehicleObjects;

		public StepCache() {
			typeCache = new Dictionary<string, GameObject[]>(); 
		}

		public void Step() {
			typeCache.Clear(); 
			_activeGameObjects = null;
			//if (_allVehicleObjects != null) {
				_allVehicleObjects = null;
			//}
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

			RemoveDestroyedObjectsFromList(_activeGameObjects);
			return _activeGameObjects;
		}

		public List<GameObject> AllVehicleObjects() {
			if (_allVehicleObjects == null) {
				List <GameObject> objs = ActiveGameObjects();
				_allVehicleObjects = new List<GameObject>();

				foreach (GameObject obj in objs) {
					GameUnit unit = obj.GameUnit();
					if (unit && unit.IsOfType(typeof(Vehicle))) {
						_allVehicleObjects.Add(obj);
					}
				}
			}

			RemoveDestroyedObjectsFromList(_allVehicleObjects);
			return _allVehicleObjects;
		}

		public List<GameUnit> AllVehicleUnits() {
			return UnitsForObjects(AllVehicleObjects());
		}


		// --- Utility ---------------------------------------

		void RemoveDestroyedObjectsFromList(List <GameObject> objs) {
			for (int i = objs.Count - 1; i >= 0; i--) {
				if (objs[i] == null || objs[i].IsDestroyed()) { 
					objs.RemoveAt(i);
				}
			}
		}

		public List<GameUnit> UnitsForObjects(List <GameObject> objs) {
			List <GameUnit> units = new List <GameUnit>();

			foreach (GameObject obj in objs) {
				GameUnit unit = obj.GameUnit();
				if (unit) {
					units.Add(unit);
				}
			}
			return units;
		}

		public List<GameObject> ObjectsForUnits(List <GameUnit> units) {
			List <GameObject> objs = new List <GameObject>();

			foreach (GameUnit unit in units) {
					objs.Add(unit.gameObject);
			}
			return objs;
		}

	}
}
