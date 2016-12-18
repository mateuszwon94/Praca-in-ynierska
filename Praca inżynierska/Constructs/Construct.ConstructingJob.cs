using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Events.Job;
using PracaInzynierska.Map;
using SFML.Graphics;
using PracaInzynierska.Utils;

namespace PracaInzynierska.Constructs
{
	public partial class Construct : Drawable {

		public class ConstructingJob : Job {
			internal ConstructingJob(Construct parent) {
				parent_ = parent;

				HashSet<MapField> set = new HashSet<MapField>();
				foreach ( MapField neighvour in parent_.Location.SelectMany(field => field.Neighbour.Where(neighvour => !parent_.Location.Contains(neighvour))) ) {
					set.Add(neighvour);
				}

				neighboursSet_ = set;
			}

			public override void Work(object sender, JobEventArgs e) {
				if ( e is ConstructingJobEventArgs ev ) { parent_.ConstructPoints += ev.Amount * 10; }
				else {
					throw new ArgumentException("JobEventArgs is not ConstructingJobEventArgs!");
				}
			}

			public override IEnumerable<MapField> Location {
				get { return neighboursSet_; }
			}

			public override Status State {
				get {
					if ( parent_.ConstructPoints <= 0f ) return Status.Planned;
					else if ( parent_.ConstructPoints < parent_.MaxConstructPoints ) return Status.Working;
					else if ( parent_.ConstructPoints >= parent_.MaxConstructPoints ) return Status.Done;

					throw new ArgumentException("This state is not allowed");
				}
			}

			private Construct parent_;

			private IEnumerable<MapField> neighboursSet_;
			private Status state_;

			public override float WorkLeft {
				get { return parent_.MaxConstructPoints - parent_.ConstructPoints; }
			}
		}
	}
}