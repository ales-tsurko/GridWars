using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp {
	public class Timer : IComparable<Timer> {
		public TimerCenter timerCenter;
		public object target;
		public string method;
		public Action action;
		public float timeout; // helper
		public float fireTime;
		public float startTime; // helper

		// optional ivars for attaching info to the timer 
		public string label; 
		public object info;

		public Timer() {
		}

		public Timer SetTimeout(float secs) {
			timeout = secs;
			return this;
		}

		public Timer SetTarget(object aTarget) {
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
			if (action == null) {
				Type targetType = target.GetType();
				MethodInfo methodInfo = targetType.GetMethod(method);
				methodInfo.Invoke(target, null);
			}
			else {
				action();
			}


			/*
			MethodInfo methodInfo = target.GetType().GetMethod(method);
			MethodInfo generic = methodInfo.MakeGenericMethod(target.GetType());
			generic.Invoke(this, null);
			*/
		}

		public Timer Start() {
			startTime = Time.time;
			fireTime = startTime + timeout;
			timerCenter.AddTimer(this);
			return this;
		}

		public float RatioDone() {
			float v = (Time.time - startTime) / timeout;
			if (v > 1) {
				v = 1;
			}
			return v;
		}

		public void Cancel() {
			timerCenter.RemoveTimer(this);
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

