using System;
using System.Collections.Generic;
using PracaInzynierska.Events.Job;
using PracaInzynierska.Map;
using PracaInzynierska.Utils.Jobs;

namespace PracaInzynierska.Beeings
{

	public partial class Men : Beeing {

		/// <summary>
		/// Praca atakowania postaci
		/// </summary>
		public class AttackJob : Job {

			/// <summary>
			/// Konstruktor pracy
			/// </summary>
			/// <param name="owner">Postać, ktora ma być atakowana</param>
			public AttackJob(Men owner) {
				owner_ = owner;
			}

			/// <summary>
			/// Miejsca z ktorych mozna atakowac
			/// </summary>
			public override IEnumerable<MapField> Location => owner_.Location.Neighbour;

			/// <summary>
			/// Ilosc pozostalej pracy do wykonania
			/// </summary>
			public override float WorkLeft => owner_.HP;

			/// <summary>
			/// Funkcja przeprowadzajaca atak w danej chwili
			/// </summary>
			/// <param name="sender">Ten kto atakuje</param>
			/// <param name="e">Argumenty ataku</param>
			public override void Work(object sender, JobEventArgs e) {
				if ( sender is Men attacker ) {
					if ( sender == owner_ ) throw new ArgumentException();

					if ( !(owner_.Job is AttackJob) ) {
						owner_.Job = attacker.AttackThis;
					}

					owner_.HP.Value -= attacker.Strength * e.Amount * 0.25f;
				}
			}

			private readonly Men owner_;
		}
	}
}
