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

		var spacing = new Vector2(itemSize.x * .2f, itemSize.y * .2f);
		var hItemCount = isVertical ? 1 : items.Count;
		var vItemCount = isVertical ? items.Count : 1;

		panel.sizeDelta = new Vector2(
			itemSize.x*hItemCount + spacing.x*(hItemCount + 1),
			itemSize.y*vItemCount + spacing.y*(vItemCount + 1)
		);

		for (int i = 0; i < items.Count; i++) {
			var item = items[i];

			if (item != null && item.GetComponent<UIButton>() != null) {
				item.GetComponent<UIButton>().SetMenuSize(itemSize);
				var rt = item.GetComponent<RectTransform>();
				rt.anchorMin = new Vector2(0.5f, 1f);
				rt.anchorMax = new Vector2(0.5f, 1f);

				Vector2 localPosition;
				if (isVertical) {
					localPosition.x = 0f;
					localPosition.y = -(itemSize.y/2 + i*(itemSize.y + spacing.y) - panel.sizeDelta.y/2);
				}
				else {
					rt.anchorMin = new Vector2(0f, 0.5f);
					rt.anchorMax = new Vector2(0f, 0.5f);
					localPosition.x = spacing.x + itemSize.x/2 + i*(itemSize.x + spacing.x) - panel.sizeDelta.x/2;
					localPosition.y = 0;
				}
				item.GetComponent<RectTransform>().localPosition = localPosition;
			}
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

    public void SetBackground(Color _color, float alpha = 1){
        backgroundColor = new Color(_color.r, _color.g, _color.b, alpha);
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
                _t.pivot = new Vector2(0.5f, 0.5f);
                _t.localScale = Vector3.one;
                _t.anchoredPosition = new Vector2(0, -_t.sizeDelta.y*.5f);
                break;
        }
    }
    public static void SetOrientation (this UIMenu _menu, MenuOrientation orientation){
        _menu.OrderMenu(orientation);
    }
}
