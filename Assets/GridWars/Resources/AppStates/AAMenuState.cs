using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;
using System.Linq;

public class AAMenuState : AppState {

    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);
        ShowAAOptionsMenu();
    }

    public override void WillExit() {
        base.WillExit();
    }

    void ShowAAOptionsMenu() {
        app.ResetMenu();
        List<AAData> aaList = new List<AAData>();
        for (int i = 0; i <= 8; i+=2) {
            aaList.Add(new AAData(){ aa = i });
        }
        foreach (AAData _data in aaList) {
            menu.AddItem(UI.MenuItem(_data.GetString(true), ChangeAA).SetData(_data));
        }
        menu.AddItem(UI.MenuItem("Back", GoBackToGraphicsMenu));
        menu.Show();
    }

    void ShowAntiAliasingMenu() {
        TransitionTo (new AAMenuState ());
    }

    void ChangeAA(){
        AAData _res = menu.selectedItem.data as AAData;
        QualitySettings.antiAliasing = _res.aa;
        App.shared.prefs.SetAA(_res.aa);
        foreach (UIButton butt in menu.items) {
            if (butt.data != null) {
                butt.SetText((butt.data as AAData).GetString());
            }
        }
        menu.selectedItem.SetText("✓" + _res.GetString());
    }

    void GoBackToGraphicsMenu(){
        TransitionTo (new GraphicsOptionsState ());
    }
}

public class AAData {
    public int aa;
    public string GetString(bool withCheckMark = false){
        string s = "";
        if (withCheckMark) {
            s += (aa == QualitySettings.antiAliasing ? "✓" : "");
        }
        if (aa == 0) {
            return s + "Off";
        } else {
            return s + aa + "x";
        }
    }
}