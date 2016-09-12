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
		Color color =  Color.Lerp(startColor, realColor, timer.RatioDone());

		//Color color = new Color(1f, 1f, 1f);
		gameObject.Paint(color);
	}

	void OnDestroy() {
		if (timer != null) { //in case start is never called.
			timer.Cancel();
		}
	}
}
