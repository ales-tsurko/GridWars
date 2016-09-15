using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrightFadeInGeneric : MonoBehaviour {

	public float period = 0.3f;

	AssemblyCSharp.Timer timer;
	ParticleSystem ps;

	private Color startColor = Color.white;
	private Dictionary<Material, Color> materialColors;

	void Start () {
		SetupMaterialColors();
	}

	void OnEnable() {
		GameObject g = gameObject;
		//print(g);
		StartTimer();
		//ShowStartValue();
	}

	void OnDisable() {
		GameObject g = gameObject;
		//print(g);
		CancelTimer();
		ShowFinalValue();
	}

	void StartTimer() {
		if (timer != null) { 
			CancelTimer();
		}
			
		timer = App.shared.timerCenter.NewTimer();
		timer.action = DestroyThisComponent;
		timer.SetTimeout(period);
		timer.Start();
	}

	void CancelTimer() {
		if (timer != null) { //in case start is never called.
			timer.Cancel();
			timer = null;
		}
	}

	void SetupMaterialColors() {
		if (materialColors == null) {
			materialColors = new Dictionary<Material, Color>();

			gameObject.EachMaterial(mat => {
				materialColors.Add(mat, mat.color);
			});
		}
	}

	void DestroyThisComponent() {
		Destroy(GetComponent<BrightFadeIn>());
		UpdateForValue(1f);
	}

	void Update () {
		float t = timer.RatioDone();
		UpdateForValue(t);
	}

	void UpdateForValue(float t) {
		SetupMaterialColors();

		gameObject.EachMaterial(mat => {
			if (!materialColors.ContainsKey(mat)) {
				print("wut");
			}
			Color realColor = materialColors[mat];
			Color currentColor = ValueForColor(realColor, t);
			mat.color = currentColor;
		});
	}

	Color ValueForColor(Color realColor, float t) {
		/*
		float r = EaseInOutSine(t, startColor.r, realColor.r - startColor.r, 1); 
		float g = EaseInOutSine(t, startColor.g, realColor.g - startColor.g, 1); 
		float b = EaseInOutSine(t, startColor.b, realColor.b - startColor.b, 1); 
		Color color = new Color(r, g, b);
		*/
		Color color =  Color.Lerp(startColor, realColor, t);
		return color;
	}

	void OnDestroy() {
		CancelTimer();
		UpdateForValue(1f);
	}


	void ShowStartValue() {
		UpdateForValue(0f);
	}

	void ShowFinalValue() {
		UpdateForValue(1f);
	}

	float EaseInOutSine(float time, float startValue, float changeInValue, float duration) {
		return - changeInValue / 2 * (Mathf.Cos(Mathf.PI * time / duration) - 1) + startValue;
	}
}
