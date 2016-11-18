using UnityEngine;
using System.Collections;

public class EnvController {

	public EnvConfig config {
		get {
			var developer = LoadConfigNamed("Developer");
			if (developer == null || !developer.hasPriority) {
				if (Application.isEditor) {
					return LoadConfigNamed("Editor");
				}
				else if (Debug.isDebugBuild) {
					return LoadConfigNamed("Debug");
				}
				else {
					return LoadConfigNamed("Release");
				}
			}
			else {
				return developer;
			}
		}
	}

	EnvConfig LoadConfigNamed(string name) {
		return Resources.Load<EnvConfig>("Environment/" + name);
	}
		
}
