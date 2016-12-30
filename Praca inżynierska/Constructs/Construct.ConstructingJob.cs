using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Beeings;
using PracaInzynierska.Events.Job;
using PracaInzynierska.Map;
using SFML.Graphics;
using PracaInzynierska.Utils;
using PracaInzynierska.Utils.Jobs;

namespace PracaInzynierska.Constructs
{
	public partial class Construct : Drawable {

		public class ConstructingJob : Job {
			internal ConstructingJob(Construct parent) {
				parent_ = parent;
			}

			public override void Work(object sender, JobEventArgs e) {
				if ( sender is Men men ) {
					parent_.ConstructPoints += e.Amount * men.Constructing * 10;
					men.Fatigue.Value -= e.Amount;
				} else throw new ArgumentException();
			}

			public override IEnumerable<MapField> Location {
				get { return parent_.Location.SelectMany(field => field.Neighbour.Where(neighvour => !parent_.Location.Contains(neighvour))).Distinct(); }
			}

			public override Status State {
				get {
					if ( parent_.ConstructPoints <= 0f ) return Status.Planned;
					else if ( parent_.ConstructPoints < parent_.MaxConstructPoints ) return Status.Working;
					else if ( parent_.ConstructPoints >= parent_.MaxConstructPoints ) return Status.Done;

					throw new ArgumentException("This state is not allowed");
				}
			}

			private readonly Construct parent_;

			public override float WorkLeft => parent_.MaxConstructPoints - parent_.ConstructPoints;
		}
	}
}