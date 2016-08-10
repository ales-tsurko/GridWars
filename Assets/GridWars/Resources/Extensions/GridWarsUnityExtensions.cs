using UnityEngine;
using System;

public static class GridWarsUnityExtensions {
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

	public static T CreateChild<T>(this MonoBehaviour self) where T: MonoBehaviour {
		var gameObject = new GameObject();
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.parent = self.transform;
		gameObject.name = typeof(T).Name;
		return gameObject.AddComponent<T>();
	}
}