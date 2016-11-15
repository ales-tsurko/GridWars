using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class ResolutionMenuState : AppState {

    private static bool _needsInitialFadeIn = true;
    Resolution[] resolutions;
    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);

        ShowGraphicsOptionsMenu();
    }

    public override void WillExit() {
        base.WillExit();
    }

    void ShowGraphicsOptionsMenu() {
        app.ResetMenu();
        resolutions = Screen.resolutions;
        List<Resolution> resList = new List<Resolution>();
        foreach (Resolution res in resolutions) {
            resList.Add(res);
        }
        Debug.Log(resList.Count);
        GameObject scroller = menu.AddNewScrollingMenu("Resolution");
        scroller.GetComponent<ResolutionScrollerController>().Display(resList);
        menu.Show();


        /*
         * 
         * string s = "";
            if (Screen.currentResolution.height == res.height && Screen.currentResolution.width == res.width) {
                s += "✓";
            }
            s += " " + res.width + "x" + res.height;
            menu.AddItem(UI.MenuItem(s, ChangeRes).SetData(res));*/


       // menu.AddItem(UI.MenuItem("Antialiasing", ShowAntiAliasingMenu));
        menu.AddItem(UI.MenuItem("Back", GoBackToGraphicsMenu));
       
       // menu.Show();

        if (_needsInitialFadeIn) {
            menu.backgroundColor = Color.black;
            menu.targetBackgroundColor = Color.clear;
            _needsInitialFadeIn = false;
        }
    }

    void ShowAntiAliasingMenu() {
        TransitionTo (new AAMenuState ());
    }

    void ChangeRes(){
        TransitionTo (new ResolutionMenuState ());
    }

    void GoBackToGraphicsMenu(){
        TransitionTo (new OptionsMenuState ());
    }

    void Cancel(){

    }
}
