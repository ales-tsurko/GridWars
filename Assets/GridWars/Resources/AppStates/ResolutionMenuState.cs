using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;
using System.Linq;

public class ResolutionMenuState : AppState {

    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);

        ShowResolutionOptionsMenu();
    }

    public override void WillExit() {
        base.WillExit();
    }

    void ShowResolutionOptionsMenu() {
        app.ResetMenu();

        if (Screen.resolutions.Length > 0) {
            var maxRes = Screen.resolutions[Screen.resolutions.Length - 1];

            foreach (var res in Screen.resolutions) {
                if (res.width < 1024 || !Mathf.Approximately(maxRes.AspectRatio(), res.AspectRatio())) {
                    continue;
                }

                menu.AddItem(UI.MenuItem(res.MenuString(), ChangeRes).SetData(new ResolutionData(){ resolution = res }));
            }
        }

        menu.AddItem(UI.MenuItem("Back", GoBackToGraphicsMenu));
        menu.Show();
    }

    void ShowAntiAliasingMenu() {
        TransitionTo (new AAMenuState ());
    }

    void ChangeRes(){
        ResolutionData _res = menu.selectedItem.data as ResolutionData;
#if UNITY_ANDROID
        var width = _res.resolution.height;
        var height = _res.resolution.width;
#else
        var width = _res.resolution.width;
        var height = _res.resolution.height;
#endif
        Screen.SetResolution(width, height, Screen.fullScreen);
		App.shared.prefs.resolution = _res.resolution;
        foreach (UIButton butt in menu.items) {
            if (butt.data != null) {
                butt.SetText((butt.data as ResolutionData).resolution.PlainString());
            }
        }
        menu.selectedItem.SetText(_res.resolution.CheckedString());
    }

    void GoBackToGraphicsMenu(){
        TransitionTo (new GraphicsOptionsState ());
    }
}

public class ResolutionData {
    public Resolution resolution;
}

public static class ResolutionExtension {
    /// <summary>
    /// Returns a formatted width X height and adds a check mark before the current screen resolution. Is rumored to watch the other methods while they sleep.
    /// </summary>
    /// <returns>Formatted String for Menu</returns>
    /// <param name="res">Res.</param>
    public static string MenuString(this Resolution res) {
		return ((App.shared.prefs.resolutionHeight == res.height && App.shared.prefs.resolutionWidth == res.width) ? "✓ " : "") + res.width + "x" + res.height;
    }
    
	public static string PlainString(this Resolution res) {
        return res.width + "x" + res.height;
    }

    public static string CheckedString(this Resolution res) {
        return "✓ " + res.width + "x" + res.height;
    }

	public static float AspectRatio(this Resolution self) {
		return (float)self.width/(float)self.height;
	}

}
