using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.System;
using System.Threading.Tasks;

namespace PracaInzynierska.Events {
	public class UpdateEventArgs : EventArgs {

		/// <summary>
		/// Typ argumentu eventu wywolywanego przy każdym obieku petli gry
		/// </summary>
		/// <param name="t">Czas jaki uplynal od ostatniego wywolania</param>
		public UpdateEventArgs(TimeSpan t) {
			UpdateTime = t.TotalSeconds;
			
		}

		/// <summary>
		/// Cas jaki uplynal od ostatniego wywolania.
		/// </summary>
		public double UpdateTime { get; private set; }

	}
}
