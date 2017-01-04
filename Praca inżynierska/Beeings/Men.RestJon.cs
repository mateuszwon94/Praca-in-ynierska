using System;
using System.Collections.Generic;
using PracaInzynierska.Events.Job;
using PracaInzynierska.Exceptions;
using PracaInzynierska.Map;
using PracaInzynierska.Utils.Jobs;

namespace PracaInzynierska.Beeings {

	public partial class Men : Beeing {

		/// <summary>
		/// Praca odpoczynku postaci
		/// </summary>
		public class RestJob : Job {

			/// <summary>
			/// Konstruktor tworzacy prace
			/// </summary>
			/// <param name="owner">Wlasciciel, ktory bedzie odpoczywac</param>
			public RestJob(Men owner) {
				owner_ = owner;
				State = Status.Planned;
			}

			/// <summary>
			/// Funkcja sprawdzająca czy pole znajduje sie w lokacji umozliwiajacej prace
			/// </summary>
			/// <param name="field">Pole, ktore chce sie sprawdzic</param>
			/// <returns>true jeśli pole jest w lokacji umozliwiajaca prace, false w przeciwnym wypadku</returns>
			public override bool IsInLocation(MapField field) => field == lastLocation_;

			/// <summary>
			/// Funkcja odopwiadajaca za wykonywanie pracy
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			public override void Work(object sender, JobEventArgs e) {
				State = Status.Working;
				if (WorkLeft <= 0f) State = Status.Done;
				else if ( sender == owner_ ) {
					owner_.HP.Value += e.Amount * 10;
					owner_.RestF.Value += e.Amount * 0.2f;
				}
				else { throw new ArgumentException(); }
			}

			/// <summary>
			/// Wlasciwosc okreslajaca ile pozostalo pracy do wykonania
			/// </summary>
			public override float WorkLeft => owner_.HP.MaxHP - owner_.HP;

			/// <summary>
			/// Iterator zwracajacy pokolei pola z ktorych mozna przeprowadzic prace
			/// </summary>
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

			/// <summary>
			/// statut pracy
			/// </summary>
			public override Status State { get; set; }

			private readonly Men owner_;

			private MapField lastLocation_;

			private static readonly Random rand_ = new Random();
		}
	}
}
