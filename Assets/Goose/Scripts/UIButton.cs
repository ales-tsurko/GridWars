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
}
