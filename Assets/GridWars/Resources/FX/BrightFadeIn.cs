using UnityEngine;
using System.Collections;

public class BrightFadeIn : MonoBehaviour {

	public float period = 0.3f;

	AssemblyCSharp.Timer timer;
	ParticleSystem ps;

	private Color startColor = Color.white;
	private Color primaryColor;
	private Color secondaryColor;

	void Start () {
		ps = GetComponent<ParticleSystem> ();
		if (ps != null) {
			return;
		}
		timer = App.shared.timerCenter.NewTimer();
		timer.action = DestroyThisComponent;
		timer.SetTimeout(period);
		timer.Start();
		GameObject g = gameObject;
		primaryColor = g.GameUnit().player.primaryColor;
		secondaryColor = g.GameUnit().player.secondaryColor;
	}

	void DestroyThisComponent() {
		Destroy(GetComponent<BrightFadeIn>());
	}

	void Update (){
		UpdateColor(primaryColor, Player.primaryColorMaterialName);
		UpdateColor(secondaryColor, Player.secondaryColorMaterialName);
	}

	void UpdateColor(Color finalColor, string materialName) {
		float t = timer.RatioDone();

		float r = EaseInOutSine(t, startColor.r, finalColor.r - startColor.r, 1); 
		float g = EaseInOutSine(t, startColor.g, finalColor.g - startColor.g, 1); 
		float b = EaseInOutSine(t, startColor.b, finalColor.b - startColor.b, 1); 
		float a = EaseInOutSine(t, startColor.a, finalColor.a - startColor.a, 1); 
		Color color = new Color(r, g, b, a);

		gameObject.Paint(color, materialName);
	}

	void OnDestroy() {
		if (timer != null) { //in case start is never called.
			timer.Cancel();
		}
		gameObject.Paint(primaryColor, Player.primaryColorMaterialName);
		gameObject.Paint(secondaryColor, Player.secondaryColorMaterialName);
	}

	float EaseInOutSine(float time, float startValue, float changeInValue, float duration) {
		return - changeInValue / 2 * (Mathf.Cos(Mathf.PI * time / duration) - 1) + startValue;
	}
}
