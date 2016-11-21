using UnityEngine;
using System.Collections;
[CreateAssetMenu]
public class DevConfig : EnvConfig {
	public bool isEnabled;

	public void CopyTo(EnvConfig config) {
		if (config.name != "Release") {
			config.serverHost = serverHost;
			config.testEndOfGameMode = testEndOfGameMode;
		}
	}
}
