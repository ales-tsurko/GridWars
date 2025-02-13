﻿using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Global Timer system which uses a single GameObject with FixedUpdate
 * to trigger a queue of timers. Timer queue is kept sorted in timeout order so
 * each step we only need to walk the front of the queue until we find a
 * timer that isn't ready to fire.
 * 
 * example use:
 *   App.shared.timerCenter.NewTimer().SetTimeout(2.0f).SetTarget(this).SetMethod("timeout").Start();
 *   
 */ 

namespace AssemblyCSharp {

	public class TimerCenter {
		List <Timer> timers;

		public bool isPaused;

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
					return;
				}
				index++;
			}

			timers.Add(newTimer);
		}

		public void Step() {
			if (isPaused) {
				return;
			}

			while (timers.Count > 0) {
				Timer timer = timers[0];
				if (timer.IsReady()) {
					try {
						timer.Send();
					}
					catch (Exception e) {
						throw e;
					}
					finally {
						timers.Remove(timer);
					}
				} else {
					// this timer isn't ready and since queue is sorted we
					// know the ones after it aren't ready either
					return;
				}
			}
		}

		public void RemoveTimersWithTarget(object aTarget) {
			for (int i = timers.Count - 1; i >= 0; i--) {
				if (timers[i].target == aTarget) { 
					timers.RemoveAt(i);
				}
			}
		}

		public void RemoveTimer(Timer timer) {
			timers.Remove(timer);
		}

		public void CancelAllTimers() {
			foreach (var timer in new List<Timer>(timers)) {
				timer.Cancel();
			}
		}

		// untested version of AddTimer which 
		// uses BinarySearch to find queue insertion point

		public void AddTimer_untested(Timer newTimer) {
			int index = timers.BinarySearch(newTimer);

			if (index > 0) {
				if (timers[index] == newTimer) {
					// already have this timer
					return;
				}

				timers.Insert(index, newTimer);

			} else {
				index = ~index; // index of first element that is larger

				if (index < timers.Count) {
					timers.Insert(index, newTimer);
				} else {
					timers.Add(newTimer);
				}

			}
		}
	}
}

