using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public static class UI {

	const string DIR = "UI/";
	const string BUTTONPREFAB = DIR + "Buttons/DefaultButton";
    const string KEYMAPBUTTONPREFAB = DIR + "Buttons/KeyMapButton";
    const string POPUP = DIR + "Popups/PopupDefault";
	const string CANVAS = "Canvas";
	const string SKINDIR = DIR + "Skins/";
	const string FONTDIR = DIR + "Fonts/";
	public const UIFont DEFAULTFONT = UIFont.LGS;

	static UIButton Button (string title, System.Action  action, MenuItemType type, string skin, bool animated, bool allcaps){
		skin += "/";
		GameObject go;
		if (animated) {
			go = MonoBehaviour.Instantiate (Resources.Load<GameObject> ("UI/AnimatedButton"));
		} else {
			go = new GameObject ();
		}
		UIButton button = go.AddComponent<UIButton> ();
		AssignToCanvas (go);
		Image image = button.GetComponent<Image> ();
		if (image == null) {
			image = button.GetComponent<Image> ();
		}
		if (type != MenuItemType.ButtonTextOnly) {
			Sprite sprite = Resources.Load<Sprite> (SKINDIR + skin + type.ToString ());
			image.overrideSprite = sprite;
		} else {
			image.overrideSprite = null;
			image.color = new Color (1f, 1f, 1f, 0f);
		}
		button.SetAction(action);
		button.SetText(title, allcaps);
		return button;
	}
    static UIButton ButtonPrefab(string title, System.Action  action){
        GameObject go = MonoBehaviour.Instantiate(Resources.Load<GameObject>(BUTTONPREFAB));
        AssignToCanvas(go);
        UIButton _button = go.GetComponent<UIButton>();
        _button.text = title;
        //_button.SizeToFit();
        _button.SetAction(action);
        return _button;
    }

    public static UIMenuItem ButtonPrefabKeyMap(KeyData _keyData, bool utilKey = false, string utilText = "", System.Action action = null){
        GameObject go = MonoBehaviour.Instantiate(Resources.Load<GameObject>(KEYMAPBUTTONPREFAB));
        AssignToCanvas(go);
        UIButtonRemapKey _button = go.GetComponent<UIButtonRemapKey>();
        if (_keyData != null) {
            _button.joyKey = _keyData.code.GetButton();
            _button.keyKey = _keyData.key;
            _button.code = _keyData.code;
        }
        if (utilKey) {
            _button.text = utilText;
        } else {
            _button.text = _keyData.description + ": " + _button.keyKey.ToString() + " or " + _button.joyKey.ToString();
        }
        if (action == null) {
            _button.SetAction(_button.OnClick);
        } else {
            _button.SetAction(action);
        }
        return _button;
    }

    public static UIPopup Popup(string title = ""){
        GameObject go = (GameObject)MonoBehaviour.Instantiate(Resources.Load<GameObject>(POPUP));
        UIPopup p = go.GetComponent<UIPopup>();
        p.SetText(title);
        AssignToCanvas (go);
        RectTransform t = go.GetComponent<RectTransform>();
        t.sizeDelta = Vector2.zero;
        return p;
    }

    public static UIMenuItem MenuItem (string title = "Button", System.Action  action = null, MenuItemType type = MenuItemType.ButtonPrefab, string skin = "Default", bool animated = true, bool allCaps = true){
        switch (type) {
            case MenuItemType.ButtonRound:
            case MenuItemType.ButtonSquare:
            case MenuItemType.ButtonTextOnly:
                return Button(title, action, type, skin, animated, allCaps);
            case MenuItemType.ButtonPrefab:
                return ButtonPrefab(title, action);   
        }
		return null;
    }

	public static UIMenu Menu (string title = "", string skin = "Default"){
		UIMenu _menu = new GameObject ().AddComponent<UIMenu> ();
		_menu.gameObject.name = "Menu";
		_menu.Init();
		AssignToCanvas (_menu.gameObject);
		//add graphic options here re skins
		_menu.SetText(title);
		return _menu;
	}

	public static UIActivityIndicator ActivityIndicator (string text = "", string skin = "Default"){
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
	}
		
	public static Font GetFont (UIFont _font) {
		//Debug.Log (FONTDIR + _font.ToString ());
		return Resources.Load<Font> (FONTDIR + _font.ToString ());
	}
	/// <summary>
	/// Returns the Canvas or creates one if null
	/// </summary>
	/// <returns>The canvas.</returns>
	public static Canvas MainCanvas () {
		GameObject go = GameObject.Find (CANVAS);
		if (go == null) {
			go = MonoBehaviour.Instantiate (Resources.Load<GameObject> ("UI/Canvas"));
			if (go == null) {
				Canvas canvas;
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
			}
		}

		go.name = CANVAS;
		return go.GetComponent<Canvas> ();
	}

	static void AssignToCanvas (GameObject go){
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
[System.Serializable]
public class UIMenuItem : UIElement {}