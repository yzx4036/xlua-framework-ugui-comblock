using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class StopwatchPowerup:TemporaryPowerup 
	{
		void Awake()
		{
			idString = "Stopwatch";
		}

		public override void RemoveEffect(RexActor actor)
		{
			if(actor.timeStop.isTimeStopped)
			{
				actor.StartTime();
			}

			base.RemoveEffect(actor);
		}

		protected override void TriggerEffect(RexActor actor)
		{
			actor.StopTime();
		}

		protected override void AnimateIn()
		{
			ScreenFlash.Instance.Flash();
		}
	}
}
