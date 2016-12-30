using System;
using System.Collections.Generic;
using PracaInzynierska.Events.Job;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;
using PracaInzynierska.Utils.Jobs;

namespace PracaInzynierska.Beeings {

	public partial class Men : Beeing {

		public class RestJob : Job {
			public RestJob(Men owner) {
				owner_ = owner;
				State = Status.Planned;
			}

			public override bool IsInLocation(MapField field) { return field == lastLocation_; }

			public override void Work(object sender, JobEventArgs e) {
				State = Status.Working;
				if (WorkLeft <= 0f) State = Status.Done;
				else if ( sender == owner_ ) {
					owner_.HP.Value += e.Amount * 10;
					owner_.Fatigue.Value += e.Amount * 0.2f;
				}
				else { throw new ArgumentException(); }
			}

			public override float WorkLeft => owner_.HP.MaxHP - owner_.HP;

			public override IEnumerable<MapField> Location {
				get {
					while (lastLocation_ == null || !lastLocation_.IsAvaliable ) {
						try {
							lastLocation_ = owner_.Location.Neighbour[rand_.Next(-5, 5), rand_.Next(-5, 5)];
						} catch ( NoSouchNeighbourException ex ) {
							continue;
						}
					}

					yield return lastLocation_;
				}
			}

			public override Status State { get; set; }

			private float wokrLeft_;

			private readonly Men owner_;

			private MapField lastLocation_;

			private static readonly Random rand_ = new Random();
		}
	}
}
