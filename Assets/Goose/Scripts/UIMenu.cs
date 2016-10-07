using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class UIMenu : UIElement {
	Image image;

	public Vector2 itemSpacing = new Vector2(18f, 18f); //TODO: match with font size?

	public List<UIButton> items = new List<UIButton> ();
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

	public UIButton selectedItem;

	public bool isNavigable = true;

	bool canNavigate {
		get {
			return isNavigable && selectableItems.Count > 0;
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

	public AudioSource audioSource;

	public void Init() {
		image = gameObject.AddComponent<Image>();
		image.color = Color.black;
		RectTransform t = GetComponent<RectTransform>();
		t.anchorMin = new Vector2(0, 0);
		t.anchorMax = new Vector2(1, 1);
		t.offsetMin = new Vector2(0, 0);
		t.offsetMax = new Vector2(0, 0);
	}

	public void AddItem (UIButton _item){
		RectTransform _i = _item.GetComponent<RectTransform> ();
        _i.SetParent(panel);
		items.Add (_item);
		_item.Show ();
		_item.menu = this;
		OrderMenu ();
	}

    public void OrderMenu (MenuOrientation orientation = MenuOrientation.Vertical, float _spacing = 0){

		bool isVertical = orientation == MenuOrientation.Vertical;
		Vector2 pivot;
		Vector2 layoutDirection;

		if (isVertical) {
			//x = 0 is center
			pivot = new Vector2(0.5f, 1f);
			layoutDirection = new Vector2(0f, -1f);
		}
		else {
			//y = 0 is center
			pivot = new Vector2(0f, 0.5f);
			layoutDirection = new Vector2(1f, 0f);
		}

		//size items equally
		var sizeEquallyItems = items.FindAll(item => item.matchesNeighborSize);
		var maxX = 0f;
		var maxY = 0f;
		foreach (var item in sizeEquallyItems) {
			var transform = item.GetComponent<RectTransform>();
			maxX = Mathf.Max(maxX, transform.sizeDelta.x);
			maxY = Mathf.Max(maxY, transform.sizeDelta.y);
		}

		foreach (var item in sizeEquallyItems) {
			item.GetComponent<RectTransform>().sizeDelta = new Vector2(maxX, maxY);
		}

		//determine panel size
		var nextPosition = Vector2.zero;
		maxX = 0f;
		maxY = 0f;
		foreach (var item in items) {
			var transform = item.GetComponent<RectTransform>();

			//*
			transform.anchorMin = pivot;
			transform.anchorMax = pivot;
			transform.pivot = pivot;

			transform.anchoredPosition = nextPosition;
			//*/

			maxX = Mathf.Max(maxX, transform.sizeDelta.x);
			maxY = Mathf.Max(maxY, transform.sizeDelta.y);

			nextPosition += Vector2.Scale(transform.sizeDelta + itemSpacing, layoutDirection);
		}

		nextPosition -= Vector2.Scale(itemSpacing, layoutDirection);

		panel.sizeDelta = new Vector2(Mathf.Max(maxX, Mathf.Abs(nextPosition.x)), Mathf.Max(maxY, Mathf.Abs(nextPosition.y)));

		this.SetAnchor(currentAnchor);
    }

    Vector2 GetMaxSizeDelta (MenuOrientation orientation) {
        bool isVertical = orientation == MenuOrientation.Vertical;

        float maxSize = 0;
        Vector2 vec = new Vector2();
        foreach (UIButton i in items) {
            if ((isVertical ? i.GetComponent<RectTransform>().sizeDelta.x : i.GetComponent<RectTransform>().sizeDelta.y)  > maxSize) {
                maxSize = (isVertical ? i.GetComponent<RectTransform>().sizeDelta.x : i.GetComponent<RectTransform>().sizeDelta.y);
                vec = i.GetComponent<RectTransform>().sizeDelta;
            }
        }
        return vec * 1.01f;
    }

	public void Reset () {
       // Destroy(gameObject);
       // return;
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}
		items = new List<UIButton> ();
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

		if (audioSource == null) {
			audioSource = gameObject.AddComponent<AudioSource>();
		}

		if (canNavigate) {
			SelectNextItem();
		}
    }

	public List<UIButton>selectableItems {
		get {
			return items.FindAll(item => item.isInteractible);
		}
	}

	public void SelectItem(UIButton item) {
		if (item != null) {
			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
			item.Select();
			selectedItem = item;
		}
	}

	public int selectedItemIndex {
		get {
			return selectableItems.IndexOf(selectedItem);
		}
	}

	public void SelectNextItem() {
		var nextIndex = selectedItemIndex + 1;
		if (nextIndex >= selectableItems.Count) {
			nextIndex = 0;
		}
		SelectItem(selectableItems[nextIndex]);
	}

	public void SelectPreviousItem() {
		var previousIndex = selectedItemIndex - 1;
		if (previousIndex < 0) {
			previousIndex = selectableItems.Count - 1;
		}
		SelectItem(selectableItems[previousIndex]);
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

	void Update() {
		if (canNavigate) {
			/* Unity calls OnClick for selected items when return is pressed by default
			if (Input.GetKeyDown(KeyCode.Return)) {
				selectedItem.OnClick();
			}
			*/

			if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
				SelectNextItem();
			}

			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {
				SelectPreviousItem();
			}
		}
	}
}

public enum MenuAnchor {MiddleCenter, TopCenter, TopLeft};
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
                _t.anchoredPosition = new Vector2(0, -_t.sizeDelta.y);
                break;
			case MenuAnchor.TopLeft:
				_t.anchorMin = new Vector2(0f, 1);
				_t.anchorMax = new Vector2(0f, 1);
				_t.pivot = new Vector2(0f, 0.5f);
				_t.localScale = Vector3.one;
				_t.anchoredPosition = new Vector2(_menu.itemSpacing.x, -_t.sizeDelta.y);
				break;
        }
    }
    public static void SetOrientation (this UIMenu _menu, MenuOrientation orientation){
        _menu.OrderMenu(orientation);
    }
        
}
