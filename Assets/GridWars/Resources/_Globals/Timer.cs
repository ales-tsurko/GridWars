using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp {
	public class Timer : IComparable<Timer> {
		public TimerCenter timerCenter;
		public object target;
		public string method;
		public float fireTime;

		// optional ivars for attaching info to the timer 
		public string label; 
		public object info;

		public Timer() {
		}

		public Timer SetTimeout(float secs) {
			fireTime = Time.time + secs;
			return this;
		}

		public Timer SetTartget(object aTarget) {
			target = aTarget;
			return this;
		}

		public Timer SetMethod(string aMethod) {
			method = aMethod;
			return this;
		}

		public bool IsReady() {
			return (Time.time >= fireTime);
		}

		public void Send() {
			Type targetType = target.GetType();
			MethodInfo methodInfo = targetType.GetMethod(method);
			methodInfo.Invoke(target, null);

			/*
			MethodInfo methodInfo = target.GetType().GetMethod(method);
			MethodInfo generic = methodInfo.MakeGenericMethod(target.GetType());
			generic.Invoke(this, null);
			*/
		}

		public void Start() {
			timerCenter.AddTimer(this);
		}


		public int CompareTo(Timer otherTimer) {
			float f1 = fireTime;
			float f2 = otherTimer.fireTime;

			if (Mathf.Approximately(f1, f2)) {
				return 0;
			}

			return f1 > f2 ? 1 : -1;
		}
	}
}

