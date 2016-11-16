using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public static class UI {

	const string DIR = "UI/";
	public const string BUTTONPREFAB = DIR + "Buttons/DefaultButton";
    const string KEYMAPBUTTONPREFAB = DIR + "Buttons/KeyMapButton";
    const string POPUP = DIR + "Popups/PopupDefault";
	const string SKINDIR = DIR + "Skins/";
	const string FONTDIR = DIR + "Fonts/";
	public const UIFont DEFAULTFONT = UIFont.LGS;

	static UIButton Button (string title, System.Action action, MenuItemType type, string skin, bool animated, bool allcaps){
		skin += "/";
		GameObject go;
		if (animated) {
			go = MonoBehaviour.Instantiate (App.shared.LoadGameObject("UI/AnimatedButton"));
			//go = MonoBehaviour.Instantiate (Resources.Load<GameObject> ("UI/AnimatedButton"));
		} else {
			go = new GameObject ();
		}
		UIButton button = go.AddComponent<UIButton> ();
		AssignToCanvas (go);
		Image image = button.GetComponent<Image> ();
		if (image == null) {
			image = button.GetComponent<Image> ();
		}
		if (type == MenuItemType.ButtonTextOnly) {
			button.allowsInteraction = false;
			button.matchesNeighborSize = false;
		} else {
			//Sprite sprite = Resources.Load<Sprite> (SKINDIR + skin + type.ToString ());
			//image.overrideSprite = sprite;
		}
		button.SetAction(action);
		button.SetText(title, allcaps);
		return button;
	}

	static UIButton ButtonPrefab(string title, System.Action  action){
		var button = UIButton.Instantiate();
		button.text = title;
		button.action = action;
		return button;
    }

    public static UIButton MenuItem (string title = "Button", System.Action  action = null, MenuItemType type = MenuItemType.ButtonPrefab, string skin = "Default", bool animated = true, bool allCaps = true){
        switch (type) {
            case MenuItemType.ButtonRound:
            case MenuItemType.ButtonSquare:
            case MenuItemType.ButtonTextOnly:
				var button = ButtonPrefab(title, null);
			button.allowsInteraction = false;
				button.matchesNeighborSize = false;
				return button;
            case MenuItemType.ButtonPrefab:
			button = ButtonPrefab(title, action);
			return button;
        }
		return null;
    }

	public static UIMenu Menu() {
		return new GameObject().AddComponent<UIMenu>();
	}

	public static UIActivityIndicator ActivityIndicator (string text = "", string skin = "Default"){
		var indicator = UIActivityIndicator.Instantiate();
		indicator.text = text;
		return indicator;
		/*
		skin += "/";
		GameObject go = new GameObject ();
		UIActivityIndicator indicator = go.AddComponent<UIActivityIndicator> ();
		AssignToCanvas (go);
		indicator.GetComponent<Image> ().overrideSprite = Resources.Load<Sprite> (SKINDIR + skin + "Activity");
		indicator.GetComponent<Image> ().color = new Color (1, 1, 1, 0);
		Text _text = indicator.SetText (text);
		_text.resizeTextForBestFit = true;
		_text.alignment = TextAnchor.LowerCenter;
		_text.rectTransform.sizeDelta = new Vector2 (Screen.width, 200);
		indicator.SetSize (400, 200);
		indicator.Hide ();
		indicator.name = "ActivityIndicator";
		return indicator;
		*/
	}
		
	public static Font GetFont (UIFont _font) {
		//Debug.Log (FONTDIR + _font.ToString ());
		return Resources.Load<Font> (FONTDIR + _font.ToString ());
	}

	static Canvas _mainCanvas;

	/// <summary>
	/// Returns the Canvas or creates one if null
	/// </summary>
	/// <returns>The canvas.</returns>
	public static Canvas MainCanvas () {
		if (_mainCanvas == null) {
			var go = MonoBehaviour.Instantiate (App.shared.LoadGameObject("UI/Canvas"));
			if (go == null) {
				Canvas canvas;
				go = new GameObject (); 
				canvas = go.AddComponent<Canvas> ();
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				//var scaler = canvas.gameObject.AddComponent<CanvasScaler> ();
				var raycaster = canvas.gameObject.AddComponent<GraphicRaycaster> ();
				raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
				var eventSystem = new GameObject ().AddComponent<EventSystem> ();
				eventSystem.gameObject.AddComponent<StandaloneInputModule> ();
				return canvas;
			}

			_mainCanvas = go.GetComponent<Canvas>();
			_mainCanvas.GetComponentInChildren<EventSystem>().sendNavigationEvents = false;

			go.name = "Canvas";
		}

		return _mainCanvas;
	}

	public static void AssignToCanvas (GameObject go){
		go.transform.SetParent (MainCanvas ().transform);
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
	}

	public static RectTransform CreateTextObj (this RectTransform _parent, UIFont _font = DEFAULTFONT){
		GameObject textObj = new GameObject ();
		var rect = textObj.AddComponent<RectTransform> ();
		rect.SetParent (_parent);
		rect.localPosition = Vector3.zero;
		rect.localScale = Vector3.one;
		rect.sizeDelta = _parent.GetComponent<RectTransform> ().sizeDelta * .5f;
		Text text = textObj.AddComponent<Text> ();
		text.resizeTextForBestFit = true;
		if (_font == UIFont.None) {
			text.font = Resources.GetBuiltinResource<Font> ("Arial.ttf");
		} else {
			text.font = GetFont (_font);
		}
		text.alignment = TextAnchor.MiddleCenter;
		return textObj.GetComponent<RectTransform> ();
	}
}

public enum MenuItemType {ButtonRound, ButtonSquare, Label, TextField, ButtonTextOnly, ButtonPrefab}
public enum UIFont {None, Army, EuroStile, LGS}