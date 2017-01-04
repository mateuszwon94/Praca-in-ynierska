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

		/// <summary>
		/// Praca odpowiadajaca ucieczce
		/// </summary>
		public class FleeJob : Job {
			/// <summary>
			/// konstruktor tworzacy ucieczke
			/// </summary>
			/// <param name="owner">Postac, ktora ma uciekac</param>
			/// <param name="map">Mapa na ktorej odbywa sie gra</param>
			/// <param name="colonosts">Kolonia</param>
			/// <param name="besigers">Oblegajacy</param>
			public FleeJob(Men owner, Map.Map map, Colony colonosts, Besiegers besigers) {
				owner_ = owner;
				colonosts_ = colonosts;
				besigers_ = besigers;
				map_ = map;
			}

			/// <summary>
			/// Wlasciwosc zwracajaca pola gdzie mozna bezpiecznie uciekac
			/// </summary>
			public override IEnumerable<MapField> Location => map_.BorderFields.Where(field => colonosts_.All(colonost => MapField.Distance(field, colonost.Location) >= map_.Size * 0.4f));

			/// <summary>
			/// Pozostala czesc pracy. Nie na sensu w tym kontekscie
			/// </summary>
			public override float WorkLeft {
				get { throw new NotImplementedException(); }
			}

			/// <summary>
			/// Status wykonywania pracy
			/// </summary>
			public override Status State {
				get { return Status.Preparation; }
				set { }
			}

			/// <summary>
			/// Wykonanie pracy ucieczki
			/// </summary>
			/// <param name="sender">obiekt, ktory ykonuje prace</param>
			/// <param name="e">Argumenty pracy</param>
			public override void Work(object sender, JobEventArgs e) => besigers_.RemoveBesiger(owner_);

			private readonly Men owner_;
			private readonly Colony colonosts_;
			private readonly Besiegers besigers_;
			private readonly Map.Map map_;
		}
	}
}
