using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class UIActivityIndicator : UIButton {
	string prefix;
	int maxDots = 10;
	int dotCount = 0;
	float showTime = 0f;

	public static new UIActivityIndicator Instantiate() {
		GameObject go = MonoBehaviour.Instantiate(Resources.Load<GameObject>(UI.BUTTONPREFAB));
		UI.AssignToCanvas(go);
		Destroy(go.GetComponent<UIButton>());
		UIActivityIndicator indicator = go.AddComponent<UIActivityIndicator>();
		return indicator;
	}

	void Awake() {
		matchesNeighborSize = false;
		isInteractible = false;
	}

	public override string text {
		get {
			return base.text;
		}

		set {
			prefix = value.ToUpper() + "\n";
			base.text = prefix;
		}
	}

	public override void Show() {
		base.Show();

		showTime = Time.time;
	}

	void Update () {
		var newDotCount = Mathf.FloorToInt(Time.time - showTime) % (maxDots + 1);

		if (dotCount != newDotCount) {
			dotCount = newDotCount;
			UpdateText();
		}

		//transform.GetComponentInChildren<Text> ().rectTransform.rotation = Quaternion.Euler (Vector3.zero);
		//transform.Rotate (transform.forward, Time.deltaTime * rotateSpeed);
	}

	void UpdateText() {
		var suffix = "";

		for (var i = 0; i < dotCount; i ++) {
			suffix += ".";
		}

		transform.GetComponentInChildren<Text>().text = prefix + suffix;
		SizeToFit();
	}

}
