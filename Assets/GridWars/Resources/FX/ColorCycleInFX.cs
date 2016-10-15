using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorCycleInFX : MonoBehaviour {

	public float startTime;
	public float lifeSpan = 1.5f; // 1.5f is length of vet sound
	public float cycles = 10f;

	private Color startColor = Color.white;
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


	float RatioDone() {
		return Mathf.Clamp(Age() / lifeSpan, 0f, 1f);
	}

	void Update() {
		// 1 is start color 0 is end color
		// we start with 0, do cycles and end at 1

		if (Age() > lifeSpan) {
			Finish();
		} else {
			float r = RatioDone();
			r = Mathf.Sqrt(r);
			r = Mathf.Sqrt(r);

			float v = 1f - (Mathf.Cos(r * Mathf.PI * (cycles * 2f - 1f)) + 1f)/2f;
			ShowRatio(v);
		}
	}

	void ShowRatio(float t) {
		SetupMaterialColorsIfNeeded();

		gameObject.EachMaterial(mat => {
			if (!materialColors.ContainsKey(mat)) {
				print("error - missing start material color");
			}
			Color realColor = materialColors[mat];
			Color currentColor = ValueForColor(realColor, t);
			mat.color = currentColor;
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
