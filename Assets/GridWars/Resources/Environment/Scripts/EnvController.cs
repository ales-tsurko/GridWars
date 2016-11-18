using UnityEngine;
using System.Collections;

public class EnvController {

	public EnvConfig config {
		get {
			if (!Debug.isDebugBuild && !Application.isEditor) {
				return LoadConfigNamed("Release");
			}
			else {
				var developer = LoadConfigNamed("Developer");
				if (developer == null || !developer.hasPriority) {
					if (Application.isEditor) {
						return LoadConfigNamed("Editor");
					}
					else if (Debug.isDebugBuild) {
						return LoadConfigNamed("Debug");
					}
					else {
						throw System.Exception("This shouldn't be possible");
					}
				}
				else {
					return developer;
				}
			}

		}
	}

	EnvConfig LoadConfigNamed(string name) {
		return Resources.Load<EnvConfig>("Environment/" + name);
	}
		
}
