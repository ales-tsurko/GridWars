using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VetAnimation : MonoBehaviour {

	public float period = 0.3f;
	public float variance = 0.0f;
	public bool useEase = true;

	AssemblyCSharp.Timer timer;
	ParticleSystem ps;

	private Color startColor = Color.white;
	private Dictionary<Material, Color> materialColors;

	void Start () {
		SetupMaterialColorsIfNeeded();
	}

	public void OnEnable() {
		//GameObject g = gameObject;
		//print(g);
		StartTimer();
		if (materialColors != null) {
			ShowStartValue();
		}
	}

	void OnDisable() {
		//GameObject g = gameObject;
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
		timer.SetTimeout(period + variance * UnityEngine.Random.value);
		timer.Start();
	}

	void CancelTimer() {
		if (timer != null) { //in case start is never called.
			timer.Cancel();
			timer = null;
		}
	}

	void SetupMaterialColorsIfNeeded() {
		if (materialColors == null) {
			materialColors = new Dictionary<Material, Color>();

			gameObject.EachMaterial(mat => {
				materialColors.Add(mat, mat.color);
			});
		}
	}

	void DestroyThisComponent() {
		UpdateForValue(1f);
		Destroy(GetComponent(this.GetType()));
	}

	void Update () {
		float t = 0; 
		if (timer != null) {
			t = timer.RatioDone();
		}
		UpdateForValue(t);

		if (Mathf.Approximately(t, 1)) {
			DestroyThisComponent();
		}
	}

	void UpdateForValue(float t) {
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

	Color ValueForColor(Color realColor, float t) {
		if (useEase) {
			float r = EaseInOutSine(t, startColor.r, realColor.r - startColor.r, 1); 
			float g = EaseInOutSine(t, startColor.g, realColor.g - startColor.g, 1); 
			float b = EaseInOutSine(t, startColor.b, realColor.b - startColor.b, 1); 
			return new Color(r, g, b);
		}
			
		return Color.Lerp(startColor, realColor, t);
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
