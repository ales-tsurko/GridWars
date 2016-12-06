using UnityEngine;
using UnityEngine.UI;

public class UITextScrollView : UIButton {

	public static new UITextScrollView Instantiate() {
		GameObject go = MonoBehaviour.Instantiate(App.shared.LoadGameObject("UI/Buttons/UITextScrollView"));
		UI.AssignToCanvas(go);
		UITextScrollView scrollView = go.GetComponent<UITextScrollView>();
		return scrollView;
	}

	public override string text {
		get {
			return textComponent.text;
		}

		set {
			textComponent.text = value;
		}
	}

	protected override void Awake () {
		base.Awake();

		gameObject.AddComponent<Button>(); //hack: so there is a selectable

		matchesNeighborSize = false;
		allowsInteraction = false;
		doesType = false;
	}

	public override void SizeToFit() {
		//menu controls this
	}
}
