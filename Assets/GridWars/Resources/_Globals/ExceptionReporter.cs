using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mindscape.Raygun4Unity;

public class ExceptionReporter {
	Dictionary<string, bool> throttleDict;

	public ExceptionReporter() {
		throttleDict = new Dictionary<string, bool>();
	}

	public void ReportException(string message, string stackTrace) {
		var key = message + stackTrace;

		if (!throttleDict.ContainsKey(key)) {
			throttleDict[key] = true;


			RaygunClient raygunClient = new RaygunClient("RAYDm54NrF6Gxo5sLot4vg==");
			raygunClient.ApplicationVersion = App.shared.releaseVersion;
			raygunClient.Send(message, stackTrace);

			//App.shared.StartCoroutine(LogException(message));
		}
	}

	IEnumerator LogException(string message) {
		yield return new WaitForEndOfFrame();
		Debug.Log(message);
	}

}
