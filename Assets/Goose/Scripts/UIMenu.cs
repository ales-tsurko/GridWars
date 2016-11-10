using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using InControl;

public class UIMenu : UIElement {
	Image image;

	public Vector2 itemSpacing = new Vector2(0f, 18f); //TODO: match with font size?

	public List<UIButton> items = new List<UIButton> ();
	public float spacing;
    private RectTransform _panel;
    [HideInInspector]
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
	UIButton previouslySelectedItem;

	public bool isNavigable = true;

	public bool selectsOnShow = true;

	public UIMenu nextMenu;
	public UIMenu previousMenu;

	public Color backgroundColor {
		get {
			if (image == null) {
				return Color.clear;
			}
			else {
				return image.color;
			}
		}

		set {
			if (image != null) {
				image.color = value;
			}
		}
	}

	public Color defaultBackgroundColor {
		get {
			//return new Color(0, 0, 0, 0.3f);
			return new Color(0, 0, 0, 0.0f);
		}
	}

	public void UseDefaultBackgroundColor() {
		backgroundColor = defaultBackgroundColor;
	}

	PlayerInputs _inputs;
	public PlayerInputs inputs {
		get {
			if (_inputs == null) {
				return App.shared.inputs;
			}
			else {
				return _inputs;
			}
		}

		set {
			_inputs = inputs;
		}
	}

	MenuOrientation _orientation;
	public MenuOrientation orientation {
		get {
			return _orientation;
		}

		set {
			var oldValue = _orientation;
			_orientation = value;
			if (oldValue != value) {
				ApplyLayout();
			}
		}
	}

	bool _isInteractible;
	public bool isInteractible {
		get {
			return _isInteractible;
		}

		set {
			_isInteractible = value;
			foreach (var item in items) {
				item.isInteractible = value;
			}
		}
	}


	//public AudioSource audioSource;

	public virtual void Awake() {
		_isInteractible = true;
		gameObject.name = "Menu";
		UI.AssignToCanvas(gameObject);
		//add graphic options here re skins
		//_menu.SetText(title);

		image = gameObject.AddComponent<Image>();
		image.raycastTarget = false;
		UseDefaultBackgroundColor();
		RectTransform t = GetComponent<RectTransform>();
		t.anchorMin = new Vector2(0, 0);
		t.anchorMax = new Vector2(1, 1);
		t.offsetMin = new Vector2(0, 0);
		t.offsetMax = new Vector2(0, 0);
	}

    public void AddItem(UIButton _item, bool isBackItem = false){
		RectTransform _i = _item.GetComponent<RectTransform> ();
        _i.SetParent(panel);
        _item.transform.localScale = Vector3.one;
		items.Add(_item);
        _item.isBackItem = isBackItem;
		_item.Show();
		_item.menu = this;
		_item.isInteractible = isInteractible;
		ApplyLayout();
	}

	public UIButton AddNewButton() {
		var button = UIButton.Instantiate();
		AddItem(button);
		return button;
	}

	public UIButton AddNewText() {
		var button = AddNewButton();
		button.allowsInteraction = false;
		button.matchesNeighborSize = false;
		return button;
	}

	public UIActivityIndicator AddNewIndicator() {
		var indicator = UIActivityIndicator.Instantiate();
		AddItem(indicator);
		return indicator;
	}

	public UIMenu SetOrientation(MenuOrientation orientation) {
		this.orientation = orientation;
		return this;
	}

    public void ApplyLayout() {
		bool isVertical = (orientation == MenuOrientation.Vertical);
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

		panel.localScale = Vector3.one;
		panel.sizeDelta = new Vector2(Mathf.Max(maxX, Mathf.Abs(nextPosition.x)), Mathf.Max(maxY, Mathf.Abs(nextPosition.y)));

		switch (anchor) {
		case MenuAnchor.MiddleCenter:
			panel.anchorMin = new Vector2(.5f, .5f);
			panel.anchorMax = new Vector2(.5f, .5f);
			panel.pivot = new Vector2(0.5f, 0.5f);
			panel.anchoredPosition = new Vector3(0f, 0f, 0f);
			break;
		case MenuAnchor.TopCenter:
			panel.anchorMin = new Vector2(.5f, 1f);
			panel.anchorMax = new Vector2(.5f, 1f);
			panel.pivot = new Vector2(0.5f, 0.5f);
			panel.anchoredPosition = new Vector2(0, -panel.sizeDelta.y);
			break;
		case MenuAnchor.TopLeft:
			panel.anchorMin = new Vector2(0f, 1f);
			panel.anchorMax = new Vector2(0f, 1f);
			panel.pivot = new Vector2(0f, 0.5f);
			panel.anchoredPosition = new Vector2(18f, -panel.sizeDelta.y);
			break;
		case MenuAnchor.TopRight:
			panel.anchorMin = new Vector2(1f, 1f);
			panel.anchorMax = new Vector2(1f, 1f);
			panel.pivot = new Vector2(1f, 0.5f);
			panel.anchoredPosition = new Vector2(-18f, -panel.sizeDelta.y);
			break;
		}
    }

    Vector2 GetMaxSizeDelta(MenuOrientation orientation) {
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

	public void Reset() {
		foreach (Transform child in panel.transform) {
			Destroy(child.gameObject);
		}
		items = new List<UIButton> ();
		previouslySelectedItem = null;
		selectedItem = null;
		ApplyLayout();
	}

	public override Text SetText(string s, bool allcaps = false, float offset = 10f, UIFont _font = UI.DEFAULTFONT) {
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
   
    public override void Show() {
        base.Show();

		this.gameObject.SetActive(true);

		if (selectsOnShow) {
			Focus();
		}

		ApplyLayout();
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
			//App.shared.Log("ItemSelected: " + item.text);
			previouslySelectedItem = selectedItem;
			selectedItem = item;
			//lastSelectionTime = Time.time;
		}
	}

	public void Focus() {
		if (canFocus) {
			if (selectedItem == null) {
				if (previouslySelectedItem == null) {
					SelectFirstItem();
				}
				else {
					SelectItem(previouslySelectedItem);
				}
			}
			else {
				SelectItem(selectedItem);
			}
		}
	}

	public void LoseFocus() {
		//Debug.Log("LoseFocus");
		if (hasFocus) {
			FindObjectOfType<CameraController>().menuHasFocus = false;
			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
			selectedItem = null;
		}
	}

	public int selectedItemIndex {
		get {
			return selectableItems.IndexOf(selectedItem);
		}
	}

	public bool hasFocus {
		get {
			return selectedItem != null;
		}
	}

	public bool canFocus {
		get {
			return selectableItems.Count > 0 && isNavigable && isActiveAndEnabled;
		}
	}

	public void ItemDeselected(UIButton item) {
		//App.shared.Log("ItemDeselected: " + item.text);
		if (selectedItem == item) {
			//App.shared.Log("selectedItem = null: " + item.text);
			selectedItem = null;
		}
	}

	public void SelectFirstItem() {
		if (selectableItems.Count > 0) {
			SelectItem(selectableItems[0]);
		}
	}

	public void SelectLastItem() {
		if (selectableItems.Count > 0) {
			SelectItem(selectableItems[selectableItems.Count - 1]);
		}
	}

	public void SelectNextItem() {
		var nextIndex = selectedItemIndex + 1;
		if (nextIndex >= selectableItems.Count) {
			if (nextMenu != null && nextMenu.canFocus) {
				StartCoroutine(this.OnEndOfFrame(() => { //otherwise menu might read input again
					nextMenu.SelectFirstItem();
				}));
				return;
			}
			else {
				nextIndex = 0;
			}
		}
		SelectItem(selectableItems[nextIndex]);
	}

	public void SelectPreviousItem() {
		var previousIndex = selectedItemIndex - 1;
		if (previousIndex < 0) {
			if (previousMenu != null && previousMenu.canFocus) {
				StartCoroutine(this.OnEndOfFrame(() => { //otherwise menu might read input again
					previousMenu.SelectLastItem();
				}));
				return;
			}
			else {
				previousIndex = selectableItems.Count - 1;
			}
		}
		SelectItem(selectableItems[previousIndex]);
	}

	public override void Hide() {
		base.Hide();

		LoseFocus();
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
    public void SetButtonTextColors(Color _color){
        foreach (UIButton b in items) {
            b.SetTextColor(_color);
        }
    }

	/*
	public float controllerPeriod = 0.25f;
	float lastSelectionTime = 0f;
	*/

	protected virtual void Update() {
		if (hasFocus) {
			if (orientation == MenuOrientation.Vertical) {
				if (inputs.upItem.WasPressed) {
					SelectPreviousItem();
				}

				if (inputs.downItem.WasPressed) {
					SelectNextItem();
				}
			}
			else {
				if (inputs.leftItem.WasPressed) {
					SelectPreviousItem();
				}

				if (inputs.rightItem.WasPressed) {
					SelectNextItem();
				}
			}

			if (inputs.selectItem.WasPressed) {
				if (selectedItem != null) {
					selectedItem.OnClick();
				}
			}

            if (inputs.goBack.WasPressed) {
                foreach (UIButton button in FindObjectsOfType<UIButton>()) {
                    if (button.isBackItem) {
                        button.OnClick();
                    }
                }
            }
				

			/*
			var controllerDirection = Input.GetAxis(controllerInputName);

			if (Time.time > lastSelectionTime + controllerPeriod) {
				if (controllerDirection < 0) {
					SelectNextItem();
				}
				else if (controllerDirection > 0) {
					SelectPreviousItem();
				}
			}
			*/
		}
	}

	MenuAnchor _anchor;
	public MenuAnchor anchor {
		get {
			return _anchor;
		}

		set {
			_anchor = value;
			ApplyLayout();
		}
	}

	public UIMenu SetAnchor(MenuAnchor anchor) {
		this.anchor = anchor;
		return this;
	}
}

public enum MenuAnchor { MiddleCenter, TopCenter, TopLeft, TopRight };
public enum MenuOrientation { Vertical, Horizontal };
