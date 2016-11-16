using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;
using System.Linq;

public class ResolutionMenuState : AppState {

    private static bool _needsInitialFadeIn = true;
   
    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);

        ShowGraphicsOptionsMenu();
    }

    public override void WillExit() {
        base.WillExit();
    }

    void ShowGraphicsOptionsMenu() {
        app.ResetMenu();
        List<Resolution> resList = new List<Resolution>();
        foreach (Resolution _res in Screen.resolutions) {
            if (_res.width <= 1024) {
                continue;
            }
            resList.Add(_res);
        }
        float ratio = resList[resList.Count - 1].width / resList[resList.Count - 1].height;
        foreach (Resolution res in resList.Where(r => Mathf.Abs((r.width/r.height)-ratio)<.1f)) {
            menu.AddItem(UI.MenuItem(res.MenuString(), ChangeRes).SetData(new ResolutionData(){ resolution = res }));
        }
        menu.AddItem(UI.MenuItem("Back", GoBackToGraphicsMenu));
        menu.Show();
    }

    void ShowAntiAliasingMenu() {
        TransitionTo (new AAMenuState ());
    }

    void ChangeRes(){
        ResolutionData _res = menu.selectedItem.data as ResolutionData;
        Screen.SetResolution(_res.resolution.width, _res.resolution.height, Screen.fullScreen);
        App.shared.prefs.SetResolution(_res.resolution);
        foreach (UIButton butt in menu.items) {
            if (butt.data != null) {
                butt.SetText((butt.data as ResolutionData).resolution.PlainString());
            }
        }
        menu.selectedItem.SetText(_res.resolution.CheckedString());
    }

    void GoBackToGraphicsMenu(){
        TransitionTo (new OptionsMenuState ());
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
    public static string MenuString(this Resolution res){
        return ((Screen.height == res.height && Screen.width == res.width) ? "✓" : "") + res.width + "x" + res.height;
    }
    public static string PlainString(this Resolution res){
        return res.width + "x" + res.height;
    }
    public static string CheckedString(this Resolution res){
        return "✓" + res.width + "x" + res.height;
    }
}
