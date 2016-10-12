using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyCSharp {

	public class StepCache {
		
		Dictionary<string, GameObject[]> typeCache;
		List<GameObject> _activeGameObjects;
		List<Vehicle> _allVehicles;
		List<GameObject> _allWreckageObjects;

		public StepCache() {
			typeCache = new Dictionary<string, GameObject[]>(); 
			_allVehicles = new List<Vehicle>();
			Reset();
		}

		public void Reset() {
			Step();
			_allVehicles.Clear();
		}

		public void Step() {
			typeCache.Clear(); 
			_activeGameObjects = null;
			_allWreckageObjects = null;
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
						//if (!obj.IsDestroyed()) {
							_activeGameObjects.Add(obj);
						//}
					}
				}
			}

			RemoveDestroyedObjectsFromList(_activeGameObjects);
			return _activeGameObjects;
		}

		public IEnumerable<GameObject> AllVehicleObjects() {
			return _allVehicles.Select<Vehicle, GameObject>(vehicle => vehicle.gameObject);
		}

		public List<Vehicle> AllVehicleUnits() {
			return _allVehicles;
		}

		public void AddVehicle(Vehicle vehicle) {
			_allVehicles.Add(vehicle);
		}

		public void RemoveVehicle(Vehicle vehicle) {
			_allVehicles.Remove(vehicle);
		}

		// --- Utility ---------------------------------------

		public List<GameObject> AllWreckageObjects() {
			if (_allWreckageObjects == null) {

				GameObject [] objs = GameObject.FindGameObjectsWithTag("Wreckage");
				_allWreckageObjects = new List<GameObject>();

				foreach (GameObject obj in objs) {
					//DestroyAfter script = obj.GetComponent<DestroyAfter>();
					//if (script != null) {
						_allWreckageObjects.Add(obj);
					//}
				}
			}

			RemoveDestroyedObjectsFromList(_allWreckageObjects);
			return _allWreckageObjects;
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
