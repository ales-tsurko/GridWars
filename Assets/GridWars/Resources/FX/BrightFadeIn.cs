using UnityEngine;
using System.Collections;

public class BrightFadeIn : MonoBehaviour {

	public float period = 0.3f;

	AssemblyCSharp.Timer timer;
	ParticleSystem ps;

	private Color startColor = Color.white;
	private Color realColor;

	void Start () {
		ps = GetComponent<ParticleSystem> ();
		if (ps != null) {
			return;
		}
		timer = App.shared.timerCenter.NewTimer();
		timer.action = DestroyThisComponent;
		timer.SetTimeout(period);
		timer.Start();
		realColor = gameObject.GameUnit().player.color;
	}

	void DestroyThisComponent() {
		Destroy(GetComponent<BrightFadeIn>());
	}

	void Update (){
		/*
		 float t = timer.RatioDone();

		float r = EaseInOutSine(t, startColor.r, realColor.r - startColor.r, 1); 
		float g = EaseInOutSine(t, startColor.g, realColor.g - startColor.g, 1); 
		float b = EaseInOutSine(t, startColor.b, realColor.b - startColor.b, 1); 
		Color color = new Color(r, g, b);

		*/
		Color color =  Color.Lerp(startColor, realColor, timer.RatioDone());
		gameObject.Paint(color);
	}

	void OnDestroy() {
		if (timer != null) { //in case start is never called.
			timer.Cancel();
		}
		gameObject.Paint(realColor);
	}

	float EaseInOutSine(float time, float startValue, float changeInValue, float duration) {
		return - changeInValue / 2 * (Mathf.Cos(Mathf.PI * time / duration) - 1) + startValue;
	}
}
