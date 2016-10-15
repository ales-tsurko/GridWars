using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorCycleFX : MonoBehaviour {

	public float startTime;
	public float delayTime = 0f;
	public float cyclePeriod = 0.5f; 
	public string onMaterialName; // if set, only apply to materials with this name

	public Color startColor = Color.white;
	private Dictionary<Material, Color> materialColors = null;

	void Start () {
		SetupMaterialColorsIfNeeded();
		startTime = Time.time;
	}

	public void OnEnable() {
	}

	void OnDisable() {
		ShowRealColors();
	}
		
	void SetupMaterialColorsIfNeeded() {
		if (materialColors == null) {
			materialColors = new Dictionary<Material, Color>();
			gameObject.EachMaterial(mat => {
				materialColors.Add(mat, mat.color);
			});
		}
	}

	void Finish() {
		ShowRealColors();
		Destroy(GetComponent(this.GetType()));
	}

	float Age() {
		return Time.time - startTime;
	}
		
	void Update() {
		//if (Age() > startTime + delayTime) {
			float t = (Age() / cyclePeriod) * 2f * Mathf.PI;
			float v = 1f - (Mathf.Sin(t) + 1f) / 2f;
			ShowRatio(v);
		//}
	}

	void ShowRatio(float t) {
		SetupMaterialColorsIfNeeded();

		gameObject.EachMaterial(mat => {
			if (!materialColors.ContainsKey(mat)) {
				print("error - missing start material color");
			}

			if (name == null || mat.name.StartsWith(onMaterialName)) {
				Color realColor = materialColors[mat];
				Color currentColor = ValueForColor(realColor, t);
				mat.color = currentColor;
			}
		});
	}

	Color ValueForColor(Color realColor, float r) {
		return Color.Lerp(startColor, realColor, r);
	}

	void OnDestroy() {
		ShowRatio(1f);
	}

	void ShowStartalue() {
		ShowRatio(0f);
	}

	void ShowRealColors() {
		SetupMaterialColorsIfNeeded();

		gameObject.EachMaterial(mat => {
			Color realColor = materialColors[mat];
			mat.color = realColor;
		});
	}
}
