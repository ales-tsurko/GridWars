using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;


public class ResolutionButton : EnhancedScrollerCellView {

    public Text text;
    public Resolution resolution;
    public void SetData (Resolution res){
        resolution = res;
        text.text = ((Screen.height == res.height && Screen.width == res.width) ? "✓" : "") + res.width + "x" + res.height;
    }

    void Update (){
        text.text = ((Screen.height == resolution.height && Screen.width == resolution.width) ? "✓" : "") + resolution.width + "x" + resolution.height;
    }

    public void OnClick () {
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        App.shared.prefs.SetResolution(resolution);
        App.shared.state.TransitionTo(new ResolutionMenuState());
    }

}
