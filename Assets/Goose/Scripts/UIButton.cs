using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class UIButton : UIMenuItem {

	UnityEvent method;
	
    public Text textComponent;

    public string text {
        get {
            return textComponent.text;
        }

        set {
            textComponent.text = value;
            SizeToFit();
        }
    }

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
		action.Invoke (this);
	}

    public void Update () {
       
    }
    IEnumerator SizeButtonToFit () {
        yield return new WaitForEndOfFrame();
        rectTransform.sizeDelta = new Vector2(textComponent.rectTransform.sizeDelta.x + 10f, rectTransform.sizeDelta.y);

    }
    public void SizeToFit() {
        StartCoroutine(SizeButtonToFit());
      //  rectTransform.sizeDelta = new Vector2(textComponent.rectTransform.sizeDelta.x + 10f, rectTransform.sizeDelta.y);
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