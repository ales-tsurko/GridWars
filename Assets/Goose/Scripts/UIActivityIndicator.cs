using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class UIActivityIndicator : UIMenuItem {
	public Image image;
	Text text;

	void Update () {
		if (Time.frameCount % 60 == 0) {
			transform.GetComponentInChildren<Text> ().text += ".";
		}
		//transform.GetComponentInChildren<Text> ().rectTransform.rotation = Quaternion.Euler (Vector3.zero);
		//transform.Rotate (transform.forward, Time.deltaTime * rotateSpeed);
	}
}
