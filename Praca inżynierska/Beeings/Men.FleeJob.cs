using System;
using System.Collections.Generic;
using System.Linq;
using PracaInzynierska.Events.Job;
using PracaInzynierska.Map;
using PracaInzynierska.Utils;
using PracaInzynierska.Utils.Jobs;
using static PracaInzynierska.Map.MapField;

namespace PracaInzynierska.Beeings {

	public partial class Men : Beeing {

		public class FleeJob : Job {
			public FleeJob(Men owner, Map.Map map, Colony colonosts, Besiegers besigers) {
				owner_ = owner;
				colonosts_ = colonosts;
				besigers_ = besigers;
				map_ = map;
			}

			public override IEnumerable<MapField> Location => map_.BorderFields.Where(field => colonosts_.All(colonost => MapField.Distance(field, colonost.Location) >= map_.Size * 0.4f));

			public override float WorkLeft {
				get { throw new NotImplementedException(); }
			}

			public override Status State {
				get { return Status.Preparation; }
				set { }
			}

			public override void Work(object sender, JobEventArgs e) => besigers_.RemoveBesiger(owner_);

			private readonly Men owner_;
			private readonly Colony colonosts_;
			private readonly Besiegers besigers_;
			private readonly Map.Map map_;
		}
	}
}
