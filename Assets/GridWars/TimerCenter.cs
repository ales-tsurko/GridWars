using System;
using System.Collections;
using System.Collections.Generic;


/*
 * example use:
 *   App.shared.timerCenter.NewTimer().SetTimeout(2.0f).SetTarget(this).SetMethod("timeout").Start();
 * 
 * 
 */ 

namespace AssemblyCSharp {
	public class TimerCenter {
		List <Timer> timers;

		public TimerCenter() {
			timers = new List<Timer>();
		}

		public Timer NewTimer() {
			Timer timer = new Timer();
			timer.timerCenter = this;

			return timer;
		}

		public void AddTimer(Timer newTimer) {
			int index = 0;
			foreach (Timer timer in timers) {
				if (newTimer.fireTime < timer.fireTime) {
					timers.Insert(index, newTimer);
					index++;
					return;
				}
			}

			timers.Add(newTimer);
		}

		public void Step() {
			while (timers.Count > 0) {
				Timer timer = timers[0];
				if (timer.IsReady()) {
					timer.Send();
					timers.RemoveAt(0);
				} else {
					return;
				}
			}
		}
	}
}

