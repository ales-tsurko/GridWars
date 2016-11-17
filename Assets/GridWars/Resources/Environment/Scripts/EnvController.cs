using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class EnvController : ScriptableObject {

    public EnvConfig debug, developer, editor, release;

    public string serverHost {
        get {
            if (!Debug.isDebugBuild) {
                return release.serverHost;
            }
            if (developer.hasPriority) {
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
            if (developer.hasPriority) {
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
            if (developer.hasPriority) {
                return developer.name;
            }
            if (Application.isEditor) {
                return editor.name;
            }
            return debug.name;
        }
    }

}
