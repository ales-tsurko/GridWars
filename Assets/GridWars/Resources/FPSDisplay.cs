using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	float lastTime;
	int count;
	int sampleCount = 60;

	public void Start() {
		TM().text = "";
	}

	TextMesh TM() {
		return GetComponent<TextMesh>();
	}

	void Update()
	{
		count++; 

		if (count == sampleCount) {
			count = 0;

			float dt = (Time.time - lastTime) / (float)sampleCount;
			int fps = Mathf.RoundToInt(1.0f / dt);
			string msg = "";

			if (fps < 100) {
				if (Application.targetFrameRate != 60) {
					msg += Mathf.Round(fps) + "/" + Application.targetFrameRate + " FPS";
				} else {
					msg += Mathf.Round(fps) + " FPS";
				}
				msg += ", " + GameObjectCount() + " objs";
			}

			TM().text = msg;
			lastTime = Time.time;
		}
	}

	int GameObjectCount() {
		var allObjects = GameObject.FindObjectsOfType(typeof(MonoBehaviour)); 
		//GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
		return allObjects.Length;
	}
}