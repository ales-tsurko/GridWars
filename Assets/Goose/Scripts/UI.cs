using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public static class UI {

	const string DIR = "UI/";
	const string BUTTON = DIR + "Button";
	const string CANVAS = "Canvas";
	const string SKINDIR = DIR + "Skins/";

	static UIButton Button (string title, System.Action<UIMenuItem> action, MenuItemType type, string skin){
		skin += "/";
		GameObject go = new GameObject ();
		UIButton button = go.AddComponent<UIButton> ();
		AssignToCanvas (go);
		Sprite sprite = Resources.Load<Sprite> (SKINDIR + skin + type.ToString());
		button.GetComponent<Image> ().overrideSprite = sprite;
		button.SetAction(action);
		button.SetText(title);
		return button;
	}

	public static UIMenuItem MenuItem (string title = "Button", System.Action<UIMenuItem> action = null, MenuItemType type = MenuItemType.ButtonRound, string skin = "Default"){
		switch (type) {
		case MenuItemType.ButtonRound:
		case MenuItemType.ButtonSquare:
			return Button (title, action, type, skin);
		}
		return null;
	}

	public static UIMenu Menu (string title = "", string skin = "Default"){
		UIMenu _menu = new GameObject ().AddComponent<UIMenu> ();
		AssignToCanvas (_menu.gameObject);
		//add graphic options here re skins
		_menu.SetText(title);
		return _menu;
	}

	public static UIActivityIndicator ActivityIndicator (string text = "", float rotateSpeed = 2, string skin = "Default"){
		skin += "/";
		GameObject go = new GameObject ();
		UIActivityIndicator indicator = go.AddComponent<UIActivityIndicator> ();
		AssignToCanvas (go);
		indicator.GetComponent<Image> ().overrideSprite = Resources.Load<Sprite> (SKINDIR + skin + "Activity");
		indicator.SetText (text);
		indicator.rotateSpeed = rotateSpeed;
		indicator.Hide ();
		indicator.name = "ActivityIndicator";
		return indicator;
	}
		
	/// <summary>
	/// Returns the Canvas or creates one if null
	/// </summary>
	/// <returns>The canvas.</returns>
	public static Canvas MainCanvas () {
		GameObject go = GameObject.Find (CANVAS);
		Canvas canvas;
		if (go == null) {
			go = new GameObject ();
			canvas = go.AddComponent<Canvas> ();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.name = CANVAS;
			//var scaler = canvas.gameObject.AddComponent<CanvasScaler> ();
			var raycaster = canvas.gameObject.AddComponent<GraphicRaycaster> ();
			raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
			var eventSystem = new GameObject ().AddComponent<EventSystem> ();
			eventSystem.gameObject.AddComponent<StandaloneInputModule> ();
			return canvas;
		} else {
			return go.GetComponent<Canvas> ();
		}
	}

	static void AssignToCanvas (GameObject go){
		go.transform.SetParent (MainCanvas ().transform);
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
	}

	public static RectTransform CreateTextObj (this RectTransform _parent){
		GameObject textObj = new GameObject ();
		var rect = textObj.AddComponent<RectTransform> ();
		rect.SetParent (_parent);
		rect.localPosition = Vector3.zero;
		rect.localScale = Vector3.one;
		rect.sizeDelta = _parent.GetComponent<RectTransform> ().sizeDelta * .5f;
		Text text = textObj.AddComponent<Text> ();
		text.resizeTextForBestFit = true;
		text.font = Resources.GetBuiltinResource<Font> ("Arial.ttf");
		text.alignment = TextAnchor.MiddleCenter;
		return textObj.GetComponent<RectTransform> ();
	}
}

public enum MenuItemType {ButtonRound, ButtonSquare, Label, TextField}
public class UIMenuItem : UIElement {}