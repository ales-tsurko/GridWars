using UnityEngine;
using System.Collections;

public class EnvController {

	public EnvConfig config {
		get {
			EnvConfig config;
			if (!Debug.isDebugBuild && !Application.isEditor) {
				config = LoadConfigNamed("Release");
			}
			else if (Application.isEditor) {
				config = LoadConfigNamed("Editor");
			}
			else {
				config = LoadConfigNamed("Debug");
			}
			
			var devConfig = Resources.Load<DevConfig>("Environment/Developer");
			if (devConfig != null && devConfig.isEnabled) {
				devConfig.CopyTo(config);
			}

			return config;
		}
	}

	EnvConfig LoadConfigNamed(string name) {
		return Resources.Load<EnvConfig>("Environment/" + name);
	}
		
}
