using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	float lastTime;

	public void Start() {
		TM().text = "";
	}

	TextMesh TM() {
		return GetComponent<TextMesh>();
	}


	void FixedUpdate()
	{
		if (lastTime > 1) {
			float dt = Time.time - lastTime;
			int fps = Mathf.RoundToInt(1.0f / dt);

			if (fps != 60) {
				TM().text = Mathf.Round(fps) + " FPS";
			} else {
				TM().text = "";
			}
		}

		lastTime = Time.time;
	}
}