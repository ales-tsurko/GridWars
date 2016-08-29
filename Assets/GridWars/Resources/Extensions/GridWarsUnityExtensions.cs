using UnityEngine;
using System;
using System.Collections.Generic;

public static class GridWarsUnityExtensions {

	public static GameUnit GameUnit(this GameObject self) {
		return self.GetComponent<GameUnit>();
	}


	public static void EachRenderer(this GameObject self, Action<MeshRenderer> f) {
		foreach (var renderer in self.GetComponentsInChildren<MeshRenderer>()) {
			f(renderer);
		}
	}

	public static void EachMaterial(this GameObject self, Action<Material> f) {
		self.EachRenderer(r => f(r.material));
	}

	public static void CloneMaterials(this GameObject self) {
		self.EachRenderer(r => r.material = new Material(r.material));
	}

	public static void Paint(this GameObject self, Color color, string materialName = null) {
		self.EachMaterial(m => {
			if (materialName == null || m.name.StartsWith(materialName)) {
				m.SetColor("_Color", color);
			}
		});
	}

	public static void DeepRemoveScripts(this GameObject self) {
		foreach (var script in self.GetComponentsInChildren<MonoBehaviour>()) {
			script.enabled = false;
			UnityEngine.Object.Destroy(script);
		}
	}

	public static T CreateChild<T>(this MonoBehaviour self) where T: MonoBehaviour {
		var gameObject = new GameObject();
		gameObject.transform.parent = self.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.name = typeof(T).Name;
		return gameObject.AddComponent<T>();
	}


	/// <summary>
	/// Checks if a GameObject has been destroyed.
	/// </summary>
	/// <param name="gameObject">GameObject reference to check for destructedness</param>
	/// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
	/// 
	public static bool IsDestroyed(this GameObject gameObject)
	{
		// UnityEngine overloads the == opeator for the GameObject type
		// and returns null when the object has been destroyed, but 
		// actually the object is still there but has not been cleaned up yet
		// if we test both we can determine if the object has been destroyed.
		return gameObject == null && !ReferenceEquals(gameObject, null);
	}

	public static bool inheritsFrom(this System.Object self, System.Type type) {
		return type.IsAssignableFrom(self.GetType());
	}
		


	// --- Fade ---------------------------

	public static void SetAlpha(this GameObject self, float newAlpha) {
		foreach (Renderer _renderer in self.GetComponentsInChildren<Renderer>() ) {
			if (_renderer) {
				foreach (Material _material in _renderer.materials) {
					if (!_material.HasProperty ("_Color")) {
						continue;
					}
					var bC = _material.color;
					_material.color = new Color(bC.r, bC.g, bC.b, newAlpha);
				}
			}
		}
	}

	/*
	public static void FadeOutStep(this GameObject self) {
		foreach (Renderer _renderer in gameObject.GetComponentsInChildren<Renderer>() ) {
			if (_renderer) {
				foreach (Material _material in _renderer.materials) {
					var bC = _material.color;
					_material.color = new Color(bC.r, bC.g, bC.b, Mathf.Lerp (bC.a, 0.0f, (Time.deltaTime * 0.01f)));
				}
			}
		}
	}
	*/

	/*
	public static List<GameUnit> UnitsForObjects(this List <GameObject> self) {
		List <GameObject> units = new List <GameObject>();

		foreach (GameObject obj in self) {
			GameUnit unit = obj.GameUnit();
			if (unit) {
				units.Add(unit);
			}
		}
		return units;
	}

	public List<GameObject> ObjectsForUnits(List <GameUnit> self) {
		List <GameObject> objs = new List <GameObject>();

		foreach (GameUnit unit in self) {
			objs.Add(unit.gameObject);
		}
		return objs;
	}
	*/
}