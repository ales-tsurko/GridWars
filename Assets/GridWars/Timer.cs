using System;
using System.Reflection;
using UnityEngine;

namespace AssemblyCSharp {
	public class Timer {
		public TimerCenter timerCenter;
		public object target;
		public string method;
		public float fireTime; 

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
				
	}
}

