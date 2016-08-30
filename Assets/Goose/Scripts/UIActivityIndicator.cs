using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class UIActivityIndicator : UIElement {
	public Image image;
	public float rotateSpeed;
	Text text;

	void Update () {
		transform.GetComponentInChildren<Text> ().rectTransform.rotation = Quaternion.Euler (Vector3.zero);
		transform.RotateAround (transform.forward, Time.deltaTime * rotateSpeed);
	}
}
