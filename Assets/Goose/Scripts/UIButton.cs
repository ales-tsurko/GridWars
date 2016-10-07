using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
[System.Serializable]
public class UIButton : UIElement {

	public static UIButton Instantiate() {
		GameObject go = MonoBehaviour.Instantiate(Resources.Load<GameObject>(UI.BUTTONPREFAB));
		UI.AssignToCanvas(go);
		UIButton button = go.GetComponent<UIButton>();
		return button;
	}

	UnityEvent method;
	
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
	}
		
	public void OnClick (){
        if (action != null) {
            action.Invoke();
        }
	}

	public void OnPointerEnter() {
		menu.SelectItem(this);
	}

	public void OnSelected() {
		isSelected = true;
	}

	public void OnDeselected() {
		isSelected = false;
	}

    public void SizeToFit() {
		var w = textComponent.preferredWidth;
		var h = textComponent.fontSize;

		rectTransform.sizeDelta = new Vector2(
			w + h*innerMargins.x*2,
			h + h*innerMargins.y*2
		);
    }

    public void SetMenuSize (Vector2 size){
        menuSize = size;
        rectTransform.sizeDelta = menuSize;
    }

    public void SetFillColor (Color _color, ButtonColorType type = ButtonColorType.Normal){
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
    }

    public void SetBorderColor (Color _color){
        Image i = transform.FindChild("Border").GetComponent<Image>();
        i.color = _color;
    }

    public void SetTextColor (Color _color){
        Text t = transform.FindChild("Text").GetComponent<Text>();
        t.color = _color;
    }
}
public enum ButtonColorType { Normal, Pressed, Hover, Disabled, All }