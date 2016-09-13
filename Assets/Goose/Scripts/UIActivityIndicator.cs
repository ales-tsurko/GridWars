using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class UIActivityIndicator : UIMenuItem {
	string prefix;
	int updateCount = 0;
	int maxDots = 10;

	public Text SetText(string text) {
		prefix = text + "\n";
		return base.SetText(prefix);
	}

	void Update () {
		if (Time.frameCount % 60 == 0) {
			UpdateText();
		}
		//transform.GetComponentInChildren<Text> ().rectTransform.rotation = Quaternion.Euler (Vector3.zero);
		//transform.Rotate (transform.forward, Time.deltaTime * rotateSpeed);
	}

	void UpdateText() {
		var suffix = "";
		var limit = updateCount % (maxDots + 1);

		for (var i = 0; i < limit; i ++) {
			suffix += ".";
		}

		transform.GetComponentInChildren<Text>().text = prefix + suffix;

		updateCount ++;
	}

}
