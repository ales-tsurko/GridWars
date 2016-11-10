using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NotificationCenter {
	Dictionary<string, List<Action<Notification>>> actionMap;

	public NotificationCenter() {
		actionMap = new Dictionary<string, List<Action<Notification>>>();
	}

	public void Add(string notificationName, Action<Notification> observerAction) {
		ActionListFor(notificationName).Add(observerAction);
	}

	public void Remove(string notificationName, Action<Notification> observerAction) {
		ActionListFor(notificationName).Remove(observerAction);
	}

	public void Post(Notification notification) {
		foreach(var observerAction in new List<Action<Notification>>(ActionListFor(notificationName))) {
			observerAction(notification);
		}
	}

	public void Post(string notificationName, object sender, object data) {
		var notification = new Notification();
		notification.sender = sender;
		notification.data = data;
		notification.name = notificationName;

		Post(notification);
	}

	public void Post(string notificationName, object sender) {
		Post(notificationName, sender, null);
	}

	List<Action<Notification>> ActionListFor(string notificationName) {
		if (!actionMap.ContainsKey(notificationName)) {
			actionMap[notificationName] = new List<Action<Notification>>();
		}

		return actionMap[notificationName];
	}
}

public class Notification {
	public string name;
	public object sender;
	public object data;
}
