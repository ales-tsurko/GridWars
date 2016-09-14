using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudButton : MonoBehaviour {
	public Text textComponent;

	public string text {
		get {
			return textComponent.text;
		}

		set {
			textComponent.text = text;
			SizeToFit();
		}
	}

	Button buttonComponent {
		get {
			return GetComponent<Button>();
		}
	}

	RectTransform rectTransform {
		get {
			return GetComponent<RectTransform>();
		}
	}

	void SizeToFit() {
		rectTransform.sizeDelta = new Vector2(textComponent.rectTransform.sizeDelta.x + 10f, rectTransform.sizeDelta.y);
	}
}
