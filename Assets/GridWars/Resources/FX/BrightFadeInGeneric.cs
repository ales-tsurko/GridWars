using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrightFadeInGeneric : MonoBehaviour {

	public float period = 0.3f;
	public float variance = 0.0f;
	public bool useEase = true;

	float startTime = 0f;

	public Color startColor = Color.white;
	private Dictionary<string, Color> materialColors = null;

	// --- enable / disable --------

	public void OnEnable() { // called when the object becomes enabled and active.
		TimerStart();
		ShowStartValue();
	}

	void OnDisable() { // called when the behaviour becomes disabled () or inactive.
		TimerCancel();
		ShowFinalValue();
	}

	// --- timer -------------------

	void TimerStart() {
		startTime = Time.time;
	}

	void TimerCancel() {
		startTime = -1f;
	}

	float TimerRatioDone() {
		if (startTime < 0) {
			return 1f;
		}

		return Mathf.Clamp((Time.time - startTime) / period, 0f, 1f);
	}
		
	bool TimerDone() {
		return TimerRatioDone() == 1f;
	}

	// --- materials ---------------------------

	void SetupMaterialColorsIfNeeded() {
		if (materialColors == null) {
			SetupMaterialColors();
		}
	}

	public void SetupMaterialColors() {
		materialColors = new Dictionary<string, Color>();

		gameObject.EachMaterial(mat => {
			if (!materialColors.ContainsKey(mat.name)) {
				materialColors.Add(mat.name, mat.color);
			}
		});
	}

	void FixedUpdate () {
		if (enabled == false) {
			return;
		}

		if (App.shared.timeCounter % 6 == 0) {
			float t = TimerRatioDone();
			UpdateForValue(t);

			if (t >= 1f) {
				enabled = false;
			}
		}
	}

	void UpdateForValue(float t) {
		SetupMaterialColorsIfNeeded(); // need to delay this until now in case object is painted w player color after init

		if (materialColors != null) {
			gameObject.EachMaterial(mat => {
				if (!materialColors.ContainsKey(mat.name)) {
					print("error - missing start material color '" + mat.name + "'");
					return;
				}
				Color realColor = materialColors[mat.name];
				Color currentColor = ValueForColor(realColor, t);
				mat.color = currentColor;
			});
		}
	}

	Color ValueForColor(Color realColor, float t) {
		if (useEase) {
			float r = EaseInOutSine(t, startColor.r, realColor.r - startColor.r, 1); 
			float g = EaseInOutSine(t, startColor.g, realColor.g - startColor.g, 1); 
			float b = EaseInOutSine(t, startColor.b, realColor.b - startColor.b, 1); 
			float a = EaseInOutSine(t, startColor.a, realColor.a - startColor.a, 1); 
			return new Color(r, g, b, a);
		}
			
		return Color.Lerp(startColor, realColor, t);
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
