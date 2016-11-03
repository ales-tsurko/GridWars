using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
[System.Serializable]
public class UIButton : UIElement {

	public static UIButton Instantiate() {
		GameObject go = MonoBehaviour.Instantiate(App.shared.LoadGameObject(UI.BUTTONPREFAB));
		UI.AssignToCanvas(go);
		UIButton button = go.GetComponent<UIButton>();
		return button;
	}

	UnityEvent method;

	public UIMenu containingMenu;
	public Text textComponent {
		get {
			return GetComponentInChildren<Text>();
		}
	}

	string _text;
    public virtual string text {
        get {
			return _text;
        }

        set {
			_text = value;
			textComponent.text = value.ToUpper();
            SizeToFit();
        }
    }

	public UIButton SetText(string text) {
		this.text = text;
		return this;
	}

	public bool isBackItem;

	public UIButton SetIsBackItem(bool isBackItem) {
		this.isBackItem = isBackItem;
		return this;
	}

	public RuntimeAnimatorController alertStyleController;

	public UIButton UseAlertStyle() {
		this.GetComponent<Animator>().runtimeAnimatorController = alertStyleController;
		return this;
	}

	public System.Action action;

	public UIButton SetAction(System.Action action) {
		this.action = action;
		return this;
	}

	public bool isInteractible {
		get {
			return buttonComponent.interactable;
		}

		set {
			buttonComponent.interactable = value;
		}
	}

	public Vector2 innerMargins = new Vector2(1, 1); //ratio of font height

    Button buttonComponent {
        get {
            return GetComponent<Button>();
        }
    }

    RectTransform rectTransform {
        get {
            return GetComponent<RectTransform>();
        }
    }
    [HideInInspector]
    public Vector2 menuSize = Vector2.zero;

	public UIMenu menu;

	public void Select() {
		buttonComponent.Select();
	}

	public bool isSelected;

    void Awake () {
		if (GetComponentInChildren<Text> () == null) {
			UI.CreateTextObj (GetComponent<RectTransform>(), UIFont.Army);
		}
		/*
        Animator[] anims = GetComponentsInChildren<Animator>();
        foreach (Animator anim in anims) {
            anim.SetTrigger("Play");
        }
        */

	}

	void Start () {
		GetComponent<Button>().onClick.AddListener(OnClick);

		var eventTrigger = gameObject.AddComponent<EventTrigger>();

		var entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Select;
		entry.callback.AddListener(data => OnSelected());
		eventTrigger.triggers.Add(entry);

		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Deselect;
		entry.callback.AddListener(data => OnDeselected());
		eventTrigger.triggers.Add(entry);

		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(data => OnPointerEnter());
		eventTrigger.triggers.Add(entry);
	}
		
	public void OnClick (){
        if (action != null) {
			App.shared.PlayAppSoundNamedAtVolume("MenuItemClicked", .5f);
            action.Invoke();
        }
	}

	public void OnPointerEnter() {
		if (isInteractible) {
			menu.SelectItem(this);
		}
	}

	public void OnSelected() {
		App.shared.PlayAppSoundNamedAtVolume("MenuItemSelected", 0.1f);
		isSelected = true;
	}

	public void OnDeselected() {
		isSelected = false;
		menu.ItemDeselected(this);
	}

    public void SizeToFit() {
		var w = textComponent.preferredWidth;

		var lineCount = textComponent.text.Split('\n').Length;
		var h = textComponent.font.lineHeight*lineCount;

		rectTransform.sizeDelta = new Vector2(
			w + textComponent.font.fontSize*innerMargins.x*2,
			h + textComponent.font.fontSize*innerMargins.y*2
		);
    }

    public void SetMenuSize (Vector2 size){
        menuSize = size;
        rectTransform.sizeDelta = menuSize;
    }

	public UIButton SetFillColor (Color _color, ButtonColorType type = ButtonColorType.Normal){
        ColorBlock c = GetComponent<Button>().colors;
        switch (type) {
            case ButtonColorType.Normal:
                c.normalColor = _color;
                break;
            case ButtonColorType.Hover:
                c.highlightedColor = _color;
                break;
            case ButtonColorType.Pressed:
                c.pressedColor = _color;
                break;
            case ButtonColorType.Disabled:
                c.disabledColor = _color;
                break;
			case ButtonColorType.All:
			SetFillColor(_color, ButtonColorType.Normal);
			SetFillColor(_color, ButtonColorType.Hover);
			SetFillColor(_color, ButtonColorType.Pressed);
			SetFillColor(_color, ButtonColorType.Disabled);
			buttonComponent.image.color = _color;
			break;
        }
        GetComponent<Button>().colors = c;
		return this;
    }

    public void SetBorderColor (Color _color){
        Image i = transform.FindChild("Border").GetComponent<Image>();
        i.color = _color;
    }

	public UIButton SetTextColor (Color _color){
        Text t = transform.FindChild("Text").GetComponent<Text>();
        t.color = _color;
		return this;
    }
}
public enum ButtonColorType { Normal, Pressed, Hover, Disabled, All }