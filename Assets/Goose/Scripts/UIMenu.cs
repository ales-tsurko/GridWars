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

	public bool selectsOnShow = true;

	public UIMenu nextMenu;
	public UIMenu previousMenu;

	public Color backgroundColor {
		get {
			if (image == null) {
				return new Color(0, 0, 0, 0);
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
				OrderMenu();
			}
		}
	}


	//public AudioSource audioSource;

	public virtual void Awake() {
		gameObject.name = "Menu";
		UI.AssignToCanvas(gameObject);
		//add graphic options here re skins
		//_menu.SetText(title);

		image = gameObject.AddComponent<Image>();
		image.raycastTarget = false;
		backgroundColor = new Color(0, 0, 0, 1);
		RectTransform t = GetComponent<RectTransform>();
		t.anchorMin = new Vector2(0, 0);
		t.anchorMax = new Vector2(1, 1);
		t.offsetMin = new Vector2(0, 0);
		t.offsetMax = new Vector2(0, 0);
	}

    public void AddItem (UIButton _item, bool isBackItem = false){
		RectTransform _i = _item.GetComponent<RectTransform> ();
        _i.SetParent(panel);
        _item.transform.localScale = Vector3.one;
		items.Add(_item);
        _item.isBackItem = isBackItem;
		_item.Show();
		_item.menu = this;
	}

	public UIButton AddNewButton() {
		var button = UIButton.Instantiate();
		button.containingMenu = this;
		AddItem(button);
		return button;
	}

	public UIButton AddNewText() {
		var button = AddNewButton();
		button.isInteractible = false;
		button.matchesNeighborSize = false;
		return button;
	}

	public UIActivityIndicator AddNewIndicator() {
		var indicator = UIActivityIndicator.Instantiate();
		indicator.containingMenu = this;
		AddItem(indicator);
		return indicator;
	}

	public UIMenu SetOrientation(MenuOrientation orientation) {
		this.orientation = orientation;
		return this;
	}

    public void OrderMenu(){
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
		foreach (Transform child in panel.transform) {
			Destroy(child.gameObject);
		}
		items = new List<UIButton> ();
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

		this.gameObject.SetActive(true);

		if (isNavigable && selectsOnShow) {
			Focus();
		}

		OrderMenu();
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
			//lastSelectionTime = Time.time;
		}
	}

	public void Focus() {
		SelectFirstItem();
	}

	public void LoseFocus() {
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		selectedItem = null;
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
			return selectableItems.Count > 0;
		}
	}

	public void ItemDeselected(UIButton item) {
		if (selectedItem == item) {
			//App.shared.Log("ItemDeselected", item);
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
			//App.shared.Log(nextMenu, this);
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

	/*
	public float controllerPeriod = 0.25f;
	float lastSelectionTime = 0f;
	*/

	protected virtual void Update() {
		if (hasFocus) {
			if (orientation == MenuOrientation.Vertical) {
				if (inputs.upItem.WasPressed) {
					//App.shared.Log("inputs.upItem.WasPressed", this);
					SelectPreviousItem();
				}

				if (inputs.downItem.WasPressed) {
					//App.shared.Log("inputs.downItem.WasPressed", this);
					SelectNextItem();
				}
			}
			else {
				if (inputs.leftItem.WasPressed) {
					//App.shared.Log("inputs.leftItem.WasPressed", this);
					SelectPreviousItem();
				}

				if (inputs.rightItem.WasPressed) {
					//App.shared.Log("inputs.rightItem.WasPressed", this);
					SelectNextItem();
				}
			}

			if (inputs.selectItem.WasPressed) {
				if (selectedItem != null) {
					//App.shared.Log("selectedItem.OnClick();", this);
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

	public void SetAnchor (MenuAnchor anchor){
		RectTransform _t = panel;
		currentAnchor = anchor;
        switch (anchor) {
            case MenuAnchor.MiddleCenter:
                _t.anchorMin = new Vector2(.5f, .5f);
                _t.anchorMax = new Vector2(.5f, .5f);
                _t.localScale = Vector3.one;
                _t.localPosition = new Vector3(0f, 0f, 0f);
                break;
            case MenuAnchor.TopCenter:
                _t.anchorMin = new Vector2(.5f, 1f);
                _t.anchorMax = new Vector2(.5f, 1f);
                _t.pivot = new Vector2(0.5f, 0.5f);
                _t.localScale = Vector3.one;
                _t.anchoredPosition = new Vector2(0, -_t.sizeDelta.y);
                break;
            case MenuAnchor.TopLeft:
                _t.anchorMin = new Vector2(0f, 1f);
                _t.anchorMax = new Vector2(0f, 1f);
                _t.pivot = new Vector2(0f, 0.5f);
                _t.localScale = Vector3.one;
                _t.anchoredPosition = new Vector2(18f, -_t.sizeDelta.y);
                break;
            case MenuAnchor.TopRight:
                _t.anchorMin = new Vector2(1f, 1f);
                _t.anchorMax = new Vector2(1f, 1f);
                _t.pivot = new Vector2(1f, 0.5f);
                _t.localScale = Vector3.one;
                _t.anchoredPosition = new Vector2(-18f, -_t.sizeDelta.y);
                break;
        }
	}
}

public enum MenuAnchor { MiddleCenter, TopCenter, TopLeft, TopRight };
public enum MenuOrientation { Vertical, Horizontal };