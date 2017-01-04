using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Beeings;
using PracaInzynierska.Constructs;
using PracaInzynierska.Events.Job;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;

namespace PracaInzynierska.Utils.Jobs {
	public class SleepJob : Job {
		public SleepJob(Men owner, Bed bed) {
			owner_ = owner;
			if ( bed != null ) {
				bed_ = bed;
				bed_.IsFree = false;
			}
		}

		public override void Work(object sender, JobEventArgs e) {
			State = Status.Working;
			if ( WorkLeft <= 0f ) {
				State = Status.Done;
				if ( bed_ != null ) bed_.IsFree = true;
			} else if ( sender == owner_ ) {
				owner_.HP.Value += e.Amount * 10;
				owner_.RestF.Value += e.Amount * (bed_ == null ? 0.33f : 0.5f);
			} else throw new ArgumentException();
		}

		public override float WorkLeft => 10f - owner_.RestF.Value;

		public override IEnumerable<MapField> Location {
			get {
				if ( bed_ == null ) {
					while ( lastLocation_ == null || !lastLocation_.IsAvaliable ) {
						try {
							lastLocation_ = owner_.Location.Neighbour[rand_.Next(-5, 5), rand_.Next(-5, 5)];
						} catch ( NoSouchNeighbourException ex ) {
							continue;
						}
					}

					yield return lastLocation_;
				} else {
					foreach ( MapField field in bed_.Location) {
						yield return field;
					}
				}
			}
		}

		private Bed bed_;
		private Men owner_;
		private MapField lastLocation_;
		private static Random rand_ = new Random();
	}
}