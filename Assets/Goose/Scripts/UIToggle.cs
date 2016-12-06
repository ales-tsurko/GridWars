using UnityEngine;
using UnityEngine.UI;

public class UIToggle : UIButton {
	public static string UIToggleChangedNotification = "UIToggleChangedNotification"; 

	public static new UIToggle Instantiate() {
		GameObject go = MonoBehaviour.Instantiate(App.shared.LoadGameObject("UI/Buttons/UIToggle"));
		UI.AssignToCanvas(go);
		UIToggle toggle = go.GetComponent<UIToggle>();
		return toggle;
	}

	public Toggle toggleComponent {
		get {
			return GetComponent<Toggle>();
		}
	}

	public override string text {
		get {
			return textComponent.text;
		}

		set {
			textComponent.text = value.ToUpper();
			SizeToFit();
		}
	}

	public bool isChecked {
		get {
			return toggleComponent.isOn;
		}
	}

	protected override void Awake () {
		base.Awake();

		matchesNeighborSize = false;
		doesType = false;
		allowsInteraction = false;
		matchesNeighborSize = false;
		toggleComponent.isOn = false;
		toggleComponent.onValueChanged.AddListener(ToggleValueChanged);
	}

	void ToggleValueChanged(bool newValue) {
		App.shared.notificationCenter.NewNotification()
		.SetName(UIToggleChangedNotification)
		.SetSender(this)
		.SetData(newValue)
		.Post();
	}

	protected override void Start() {
		base.Start();
	}

	public override void SizeToFit() {
		var bg = transform.Find("Background").GetComponent<RectTransform>();
		var label = transform.Find("Label").GetComponent<RectTransform>();
		label.sizeDelta = new Vector2(textComponent.preferredWidth, label.sizeDelta.y);

		GetComponent<RectTransform>().sizeDelta = new Vector2(
			label.anchoredPosition.x + label.sizeDelta.x + textComponent.font.fontSize*innerMargins.x*2,
			Mathf.Max(bg.sizeDelta.y, label.sizeDelta.y) +textComponent.font.fontSize*innerMargins.y*2
		);
	}
}
