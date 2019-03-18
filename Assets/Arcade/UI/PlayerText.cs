using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerText : MonoBehaviour
{
    public Text text;
    public float fadeDuration = 3f;

    public void FadeInWithText(string text) {
        SetText(text);
        FadeIn();
    }

    public void FadeOutWithText(string text) {
        SetText(text);
        FadeOut();
    }

    public void SetText(string text) {
        this.text.text = text;
    }

    public void FadeIn() {
        text.canvasRenderer.SetAlpha(0.0f);
        text.CrossFadeAlpha(1.0f, fadeDuration, true);
    }

    public void FadeOut() {
        text.canvasRenderer.SetAlpha(1f);
        text.CrossFadeAlpha(0f, fadeDuration, true);
    }
}
