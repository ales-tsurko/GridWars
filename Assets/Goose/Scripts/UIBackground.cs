using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class UIBackground : UIMenuItem {

    public Color color;
    RectTransform _t;
    public void Init(Color _color){
        _t = GetComponent<RectTransform>();
        color = _color;
        Image image = GetComponent<Image>();
        image.overrideSprite = null;
        image.color = _color;
        GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height) * 3f;
        _t.SetAsFirstSibling();
        _t.localPosition = Vector3.zero;
    }

    public void Update() {
        if (Time.frameCount % 10 == 0) {
            _t.SetAsFirstSibling();
            _t.localPosition = Vector3.zero;
        }
    }

}
