using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Events.Job;
using PracaInzynierska.Map;
using Priority_Queue;

namespace PracaInzynierska.Utils.Jobs {
	public abstract class Job : FastPriorityQueueNode {

		public abstract void Work(object sender, JobEventArgs e);

		public virtual bool IsInLocation(MapField field) { return Location.Contains(field); }

		public abstract float WorkLeft { get; }

		public virtual Status State { get; set; }

		public abstract IEnumerable<MapField> Location { get; }

		public enum Status {
			Planned = 0,
			Preparation = 1,
			Working = 2,
			Paused = 3,
			Done = 4,
		}

	}
}
