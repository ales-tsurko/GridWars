using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class UIMenu : UIElement {
	Image image;

	public List<UIMenuItem> items = new List<UIMenuItem> ();
	public float spacing;
    private RectTransform _panel;
    [HideInInspector]
    public  MenuAnchor currentAnchor = MenuAnchor.MiddleCenter;
    public RectTransform panel {
        get {
            if (_panel == null) {
                GameObject go = new GameObject();
                go.name = "Panel";
                go.AddComponent<CanvasRenderer>();
                _panel = go.AddComponent<RectTransform>();
                go.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
                _panel.localPosition = Vector3.zero;
                _panel.localScale = Vector3.one;
            }
            return _panel;
        }
    }

	public Color backgroundColor {
		get {
			return image.color;
		}

		set {
			image.color = value;
		}
	}

	public void Init() {
		image = gameObject.AddComponent<Image>();
		image.color = Color.black;
		RectTransform t = GetComponent<RectTransform>();
		t.anchorMin = new Vector2(0, 0);
		t.anchorMax = new Vector2(1, 1);
		t.offsetMin = new Vector2(0, 0);
		t.offsetMax = new Vector2(0, 0);
	}

	public void AddItem (UIMenuItem _item){
		RectTransform _i = _item.GetComponent<RectTransform> ();
        _i.SetParent(panel);
		items.Add (_item);
		_item.Show ();
		OrderMenu ();
	}

    public void OrderMenu (MenuOrientation orientation = MenuOrientation.Vertical, float _spacing = 0){
		bool isVertical = orientation == MenuOrientation.Vertical;
		Vector2 itemSize = GetMaxSizeDelta(orientation);

		panel.sizeDelta = new Vector2(itemSize.x * 1.2f * (isVertical ? 1 : items.Count), itemSize.y * 1.2f * (!isVertical ? 1 : items.Count));
		if (_spacing <= 0) {
			_spacing = isVertical ? itemSize.y * .2f : itemSize.x * .2f;
		}
		spacing = _spacing;
		for (var i = 0; i < items.Count; i ++) {
			var item = items[i];

			var _rect = item.GetComponent<RectTransform>();
			_rect.anchorMin = new Vector2(.5f, (isVertical ? .5f : .5f));//in case we need to change this for horiz
			_rect.anchorMax = new Vector2(.5f, (isVertical ? .5f : .5f));

			var button = item.GetComponent<UIButton>();
			if (button != null) {
				button.SetMenuSize(itemSize);
			}
			item.GetComponent<RectTransform>().localPosition = new Vector2((isVertical ? 0 : (-((_panel.sizeDelta.x - itemSize.x) * .5f) + (i * (spacing + itemSize.x)))), (!isVertical ? 0 : (((_panel.sizeDelta.y - itemSize.y) * .5f) - (i * (spacing + itemSize.y)))));//-(i * (itemSize.y + spacing))
		}

		this.SetAnchor(currentAnchor);
    }

    Vector2 GetMaxSizeDelta (MenuOrientation orientation) {
        bool isVertical = orientation == MenuOrientation.Vertical;

        float maxSize = 0;
        Vector2 vec = new Vector2();
        foreach (UIMenuItem i in items) {
            if ((isVertical ? i.GetComponent<RectTransform>().sizeDelta.x : i.GetComponent<RectTransform>().sizeDelta.y)  > maxSize) {
                maxSize = (isVertical ? i.GetComponent<RectTransform>().sizeDelta.x : i.GetComponent<RectTransform>().sizeDelta.y);
                vec = i.GetComponent<RectTransform>().sizeDelta;
            }
        }
        return vec * 1.01f;
    }

	public void Reset () {
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}
		items = new List<UIMenuItem> ();
	}

	public override Text SetText (string s, bool allcaps = false, float offset = 10f, UIFont _font = UI.DEFAULTFONT) {
		Text textObj = null;
		RectTransform _t = GetComponent<RectTransform> ();
		textObj = GetComponentInChildren<Text> ();	
		if (textObj == null) {
			textObj = UI.CreateTextObj (_t).GetComponent<Text>();
		}
		textObj.GetComponent<RectTransform> ().localPosition = new Vector2 (0, (GetComponent<RectTransform> ().sizeDelta.y * .5f / 2) - offset);
		if (allcaps) {
			s = s.ToUpper ();
		}
		textObj.text = s;
		gameObject.name = s;
		return textObj;
	}
    public override void Show () {
        base.Show();
        RectTransform t = GetComponent<RectTransform>();
        t.anchorMin = new Vector2(0, 0);
        t.anchorMax = new Vector2(1, 1);
        t.offsetMin = new Vector2(0, 0);
        t.offsetMax = new Vector2(0, 0);
    }
    public void SetButtonFillColors(Color _color, ButtonColorType type = ButtonColorType.Normal){
        foreach (UIButton b in items) {
            b.SetFillColor(_color, type);
        }
    }
    public void SetButtonBorderColors(Color _color){
        foreach (UIButton b in items) {
            b.SetBorderColor(_color);
        }
    }
    public void SetButtonTextColors (Color _color){
        foreach (UIButton b in items) {
            b.SetTextColor(_color);
        }
    }

}

public enum MenuAnchor {MiddleCenter, TopCenter};
public enum MenuOrientation {Vertical, Horizontal};
public static class UIMenuExtension {
    public static void SetAnchor (this UIMenu _menu, MenuAnchor anchor){
        RectTransform _t = _menu.panel;
        _menu.currentAnchor = anchor;
        switch (anchor) {
            case MenuAnchor.MiddleCenter:
                _t.anchorMin = new Vector2(.5f, .5f);
                _t.anchorMax = new Vector2(.5f, .5f);
                _t.localPosition = new Vector3(0f, 0f, 0f);
                break;
            case MenuAnchor.TopCenter:
                _t.anchorMin = new Vector2(.5f, 1);
                _t.anchorMax = new Vector2(.5f, 1);
                _t.pivot = new Vector2(0, 0);
                _t.localScale = Vector3.one;
                _t.anchoredPosition = new Vector2(0, -_t.sizeDelta.y*.5f);
                break;
        }
    }
    public static void SetOrientation (this UIMenu _menu, MenuOrientation orientation){
        _menu.OrderMenu(orientation);
    }
}
