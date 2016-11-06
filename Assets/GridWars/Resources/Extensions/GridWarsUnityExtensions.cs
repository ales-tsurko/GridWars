using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class GridWarsUnityExtensions {

	/*
	public static GameObject FindObject(this GameObject parent, string name) {
		Transform[] trs = parent.GetComponentsInChildren(typeof(Transform), true);

		foreach(Transform t in trs) {
			if(t.name == name) {
				return t.gameObject;
			}
		}

		return null;
	}
	*/

	/*
	public static void HideMesh(this GameObject self){
		self.GetComponent<MeshFilter>().enabled = false;
	}

	public static void UnhideMesh(this GameObject self){
		self.GetComponent<MeshFilter>().enabled = true;
	}
	*/

	public static GameUnit GameUnit(this GameObject self) {
		return self.GetComponent<GameUnit>();
	}

	// List

	/*
	public static T IList<T> Shuffled<T>(this IList<T> list) {		
		return list.OrderBy(a => UnityEngine.Random.value);
	}
	*/

	public static List<T> Shuffled<T>(this IList<T> list) {  
		List<T> input = new List<T>(list);
		List<T> output = new List<T>();

		int max = list.Count;
		while (input.Count > 0) {
			int j = (int)(UnityEngine.Random.value * (float)input.Count);
			var value = input[j];
			input.RemoveAt(j);
			output.Add(value);
		}

		return output;
	}
		
	public static void AddIfAbsent<T>(this IList<T> self, T v) {
		if (self.Contains(v) == false) {
			self.Add(v);
		}
	}

	public static T PickRandom<T>(this IList<T> self)
	{
		if (self.Count == 0) {
			return default (T);
		}

		int i = (int)Mathf.Floor(self.Count * UnityEngine.Random.value);
		return self[i];
	}

	// Rendering

	public static void EachRenderer(this GameObject self, Action<MeshRenderer> f) {
		/*
		if (self.GameUnit() && self.GameUnit().GetType() == typeof(Tank)) {
			Debug.Log("paint tank");
		}
		*/

		/*
		foreach (var renderer in self.GetComponentsInChildren<MeshRenderer>()) {
			f(renderer);
		}
		*/


		MeshRenderer[] renderers = self.GetComponentsInChildren<MeshRenderer>();
		int max = renderers.Length;
		for (int i = 0; i < max; i++) {
			MeshRenderer renderer = renderers[i];
			f(renderer);
		}
	}

	public static void EachMaterial(this GameObject self, Action<Material> f) {
		self.EachRenderer(r => f(r.material));
	}

	public static void CloneMaterials(this GameObject self) {
		self.EachRenderer(r => r.material = new Material(r.material));
	}

	public static void TurnOffShadows(this GameObject self) {
		self.EachRenderer(r => r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off);
	}

	public static void Paint(this GameObject self, Color color, string materialName = null) {
		self.EachMaterial(m => {
			if (materialName == null || m.name.StartsWith(materialName)) {
				m.SetColor("_Color", color);
			}
		});
	}

	public static void PaintDarken(this GameObject self, float v) {
		self.EachMaterial(m => {
			Color c = m.color;
			float r = c.r * v;
			float g = c.g * v;
			float b = c.b * v;
			m.SetColor("_Color", new Color(r, g, b, 1f));
			//m.SetColor("_Color", new Color(c.r, c.g, c.b, v));
		});
	}

	public static void DeepRemoveScripts(this GameObject self) {
		foreach (var script in self.GetComponentsInChildren<MonoBehaviour>()) {
			script.enabled = false;
			UnityEngine.Object.Destroy(script);
		}
	}

	public static float RotY(this GameObject obj) {
		return obj.transform.eulerAngles.y;
	}

	public static void SetRotY(this GameObject obj, float a) {
		var e = obj.transform.eulerAngles;
		obj.transform.eulerAngles = new Vector3(e.x, a, e.z);
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

	public static IEnumerator OnEndOfFrame(this MonoBehaviour self, Action action) {
		yield return new WaitForEndOfFrame();
		action();
	}

	/// <summary>
	/// Checks if a GameObject has been destroyed.
	/// </summary>
	/// <param name="gameObject">GameObject reference to check for destructedness</param>
	/// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
	/// 
	public static bool IsDestroyed(this GameObject self)
	{
		// UnityEngine overloads the == opeator for the GameObject type
		// and returns null when the object has been destroyed, but 
		// actually the object is still there but has not been cleaned up yet
		// if we test both we can determine if the object has been destroyed.
		//return gameObject == null && !ReferenceEquals(gameObject, null);

		var gameUnit = self.GetComponent<GameUnit>();
		return (gameUnit != null) && !gameUnit.isInGame;
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

	// Keyboard

	public static string FormatForKeyboard (this string s){
		if (s.StartsWith ("Alpha")) {
			return s.Remove (0, 5);
		}
		return s;
	}

	//Colors

	public static Color WithH(this Color self, float h) {
		float ch;
		float cs;
		float cv;

		Color.RGBToHSV(self, out ch, out cs, out cv);

		return Color.HSVToRGB(h, cs, cv);
	}

	public static Color WithS(this Color self, float s) {
		float ch;
		float cs;
		float cv;

		Color.RGBToHSV(self, out ch, out cs, out cv);

		return Color.HSVToRGB(ch, s, cv);
	}

	public static Color WithV(this Color self, float v) {
		float ch;
		float cs;
		float cv;

		Color.RGBToHSV(self, out ch, out cs, out cv);
		return Color.HSVToRGB(ch, cs, v);
	}

	public static Color ToP1Green(this Color self) {
		self.r = 65f/255;
		self.g = 255f/255;
		self.b = 0f/255;
		return self;
	}

	public static Color ToP3Amber(this Color self) {
		self.r = 255f/255;
		self.g = 168f/255;
		self.b = 0f/255;
		return self;
	}

	public static Color ToP11Blue(this Color self) {
		self.r = 0f/255;
		self.g = 102f/255;
		self.b = 255f/255;
		return self;
	}

	public static Color ToTronTerminalBlue(this Color self) {
		self.r = 88f/255;
		self.g = 144f/255;
		self.b = 251f/255;
		return self;
	}

	public static Color ToCyan(this Color self) {
		self.r = 0f/255;
		self.g = 255f/255;
		self.b = 255f/255;
		return self;
	}

	public static Color ToMagenta(this Color self) {
		self.r = 255f/255;
		self.g = 0f/255;
		self.b = 255f/255;
		return self;
	}

	public static Bounds BoxBounds(this GameObject self) {
		BoxCollider bc = self.GetComponent<BoxCollider>();
		Bounds b = new Bounds();
		b.center = bc.center;
		b.extents = bc.size;
		return b;
	}

	public static string Repeat(this string s, int n)
	{
		return new String(Enumerable.Range(0, n).SelectMany(x => s).ToArray());
	}

	public static string Capitalized(this string self) {
		return self.Substring(0, 1).ToUpper() + self.Substring(1);
	}

	/*
	public static string Repeat(this char c, int n)
	{
		return new String(c, n);
	}
	*/
}