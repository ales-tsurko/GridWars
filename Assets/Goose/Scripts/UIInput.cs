using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class UIInput : UIButton {

	public static new UIInput Instantiate() {
		GameObject go = MonoBehaviour.Instantiate(App.shared.LoadGameObject("UI/Buttons/UIInput"));
		UI.AssignToCanvas(go);
		UIInput input = go.GetComponent<UIInput>();
		return input;
	}

	public bool capitalizes = false;

	public InputField inputComponent {
		get {
			return GetComponent<InputField>();
		}
	}

	public override string text {
		get {
			return inputComponent.text;
		}

		set {
			if (capitalizes) {
				inputComponent.text = value.ToUpper();
			}
			else {
				inputComponent.text = value;
			}

		}
	}

	public int characterLimit {
		get {
			return inputComponent.characterLimit;
		}

		set {
			inputComponent.characterLimit = value;
			SizeToFit();
		}
	}

	protected override void Awake () {
		base.Awake();

		matchesNeighborSize = false;
		doesType = false;
		characterLimit = 64;
	}

	protected override void Start() {
		base.Start();
	}

	public override void SizeToFit() {
		var w = textComponent.font.fontSize*inputComponent.characterLimit;
		var h = textComponent.font.lineHeight;

		rectTransform.sizeDelta = new Vector2(
			w + textComponent.font.fontSize*innerMargins.x*2,
			h + textComponent.font.fontSize*innerMargins.y*2
		);

		textComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(
			w + textComponent.font.fontSize*innerMargins.x*2,
			h + textComponent.font.fontSize*innerMargins.y*2
		);

		if (menu != null) {
			menu.ApplyLayout();
		}
	}

	public void OnValueChanged() {
		this.text = text; //apply text transforms
	}

	public void OnEndEdit() {
	}
}
