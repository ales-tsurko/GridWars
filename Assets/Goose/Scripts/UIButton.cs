using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
[System.Serializable]
public class UIButton : UIMenuItem {

	UnityEvent method;
	
    public Text textComponent;

	string _text;
    public string text {
        get {
			return _text;
        }

        set {
			_text = value;
			textComponent.text = value.ToUpper();
            SizeToFit();
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

    void Awake () {
		if (GetComponentInChildren<Text> () == null) {
			UI.CreateTextObj (GetComponent<RectTransform>(), UIFont.Army);
		}
        Animator[] anims = GetComponentsInChildren<Animator>();
        foreach (Animator anim in anims) {
            anim.SetTrigger("Play");
        }

	}

	void Start () {
		GetComponent<Button> ().onClick.AddListener (() => OnClick ());
	}
		
	public void OnClick (){
        if (action != null) {
            action.Invoke();
        }
	}

    public void Update () {
       
    }

    public void SizeToFit() {
		//textComponent.
		//var settings = textComponent.GetGenerationSettings(new Vector2(float.MaxValue, float.MaxValue));
		//var settings = textComponent.GetGenerationSettings(new Vector2(1920f, 1080f));
		//var settings = textComponent.GetGenerationSettings(textComponent.rect);
		//var w = textComponent.cachedTextGenerator.GetPreferredWidth(textComponent.text, settings);
		//var h = textComponent.cachedTextGenerator.GetPreferredHeight(textComponent.text, settings);
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
public enum ButtonColorType {Normal, Pressed, Hover, Disabled}