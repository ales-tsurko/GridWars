using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NotificationCenter {
	Dictionary<string, List<Observation>> observationMap;

	public NotificationCenter() {
		observationMap = new Dictionary<string, List<Observation>>();
	}

	public Observation NewObservation() {
		var observation = new Observation();
		observation.notificationCenter = this;
		return observation;
	}

	public void Add(Observation observation) {
		ObservationListFor(observation.notificationName).Add(observation);
	}

	public void Remove(Observation observation) {
		ObservationListFor(observation.notificationName).Remove(observation);
	}

	public void RemoveObserver(object observer) {
		foreach (var pair in observationMap) {
			foreach (var observation in pair.Value.Copied()) {
				if (observation.action.Target == observer) {
					Remove(observation);
				}
			}
		}
	}

	public Notification NewNotification() {
		var notification = new Notification();
		notification.notificationCenter = this;
		return notification;
	}

	public void Post(Notification notification) {
		foreach(var observation in new List<Observation>(ObservationListFor(notification.name))) {
			if (observation.sender == notification.sender || observation.sender == null) {
				observation.action(notification);
			}
		}
	}

	List<Observation> ObservationListFor(string notificationName) {
		if (!observationMap.ContainsKey(notificationName)) {
			observationMap[notificationName] = new List<Observation>();
		}

		return observationMap[notificationName];
	}
}

public class Notification {
	public NotificationCenter notificationCenter;

	public string name;
	public Notification setName(string name) {
		this.name = name;
		return this;
	}

	public object sender;
	public Notification setSender(object sender) {
		this.sender = sender;
		return this;
	}

	public object data;
	public Notification setData(object data) {
		this.data = data;
		return this;
	}

	public void Post() {
		notificationCenter.Post(this);
	}
}

public class Observation {
	public NotificationCenter notificationCenter;

	public string notificationName;
	public Observation SetNotificationName(string notificationName) {
		this.notificationName = notificationName;
		return this;
	}

	public object sender;
	public Observation SetSender(object sender) {
		this.sender = sender;
		return this;
	}

	public Action<Notification> action;
	public Observation SetAction(Action<Notification> action) {
		this.action = action;
		return this;
	}

	public void Add() {
		notificationCenter.Add(this);
	}

	public void Remove() {
		notificationCenter.Remove(this);
	}
}
