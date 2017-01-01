using System;
using System.Collections.Generic;
using PracaInzynierska.Events.Job;
using PracaInzynierska.Map;
using PracaInzynierska.Utils.Jobs;

namespace PracaInzynierska.Beeings
{

	public partial class Men : Beeing {

		public class AttackJob : Job {
			public AttackJob(Men owner) {
				owner_ = owner;
			}
			public override IEnumerable<MapField> Location => owner_.Location.Neighbour;

			public override float WorkLeft => owner_.HP;

			public override void Work(object sender, JobEventArgs e) {
				if ( sender is Men attacker ) {
					if ( sender == owner_ ) throw new ArgumentException();

					if ( !(owner_.Job is AttackJob) ) {
						if (owner_.Colony != null && owner_.Job != null) owner_.Colony.JobQueue.Enqueue(owner_.Job, 0.5f);
						owner_.Job = attacker.AttackThis;
					}

					owner_.HP.Value -= attacker.Strength * e.Amount * 0.25f;
				}
			}

			private Men owner_;
		}
	}
}
