using UnityEngine;
using System;

public static class GridWarsUnityExtensions {
	public static void EachRenderer(this MonoBehaviour self, Func<MeshRenderer, Void> f) {
		foreach (var renderer in self.GetComponentsInChildren<MeshRenderer>()) {
			f(renderer);
		}
	}

	public static void EachMaterial(this MonoBehaviour self, Func<Material, Void> f) {
		self.EachRenderer(r => f(r.material));
	}
}