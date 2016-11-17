using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class EnvController : ScriptableObject {

    public EnvConfig debug, developer, editor, release;
    public bool developerMode;

    public string host {
        get {
            if (!Debug.isDebugBuild) {
                return release.serverHost;
            }
            if (developerMode) {
                return developer.serverHost;
            }
            if (Application.isEditor) {
                return editor.serverHost;
            }
            return debug.serverHost;
        }
    }

    public string prefsPrefix {
        get {
            if (!Debug.isDebugBuild) {
                return release.prefsPrefix;
            }
            if (developerMode) {
                return developer.prefsPrefix;
            }
            if (Application.isEditor) {
                return editor.prefsPrefix;
            }
            return debug.prefsPrefix;
        }
    }

    public string envName {
        get {
            if (!Debug.isDebugBuild) {
                return release.name;
            }
            if (developerMode) {
                return developer.name;
            }
            if (Application.isEditor) {
                return editor.name;
            }
            return debug.name;
        }
    }

}
