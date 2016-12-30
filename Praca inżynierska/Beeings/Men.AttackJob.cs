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

					owner_.HP.Value -= attacker.Strength * e.Amount * 0.25f;
				}
			}

			private Men owner_;
		}
	}
}
