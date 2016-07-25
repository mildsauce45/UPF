using System;
using UnityEngine;

namespace FirstWave.Unity.Core.Utilities
{
	public class Timer : ITimer
	{
		public float TimeSpan { get; set; }
		public Action TickAction { get; set; }
		public bool Enabled { get; set; }

		public virtual bool IsComplete { get; set; }

		private float passedTime { get; set; }

		public Timer(float timeSpan)
		{
			TimeSpan = timeSpan;
		}

		public Timer(float timeSpan, Action tickAction)
			: this(timeSpan)
		{
			TickAction = tickAction;
		}

		public void Start()
		{
			if (!Enabled)
			{
				Enabled = true;
				Reset();
			}
		}

		public void Stop()
		{
			if (Enabled)
				Enabled = false;
		}

		public virtual void Update()
		{
			if (!Enabled)
				return;

			passedTime += Time.deltaTime;

			if (passedTime >= TimeSpan)
			{
				passedTime = 0f;
				IsComplete = true;

				if (TickAction != null)
					TickAction();
			}
		}

		public virtual void Reset()
		{
			passedTime = 0f;
			IsComplete = false;
		}
	}
}
