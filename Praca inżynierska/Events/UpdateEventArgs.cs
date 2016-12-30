using System;

namespace PracaInzynierska.Events {
	public class UpdateEventArgs : EventArgs {

	    /// <summary>
	    /// Typ argumentu eventu wywolywanego przy każdym obieku petli gry
	    /// </summary>
	    /// <param name="t">Czas jaki uplynal od ostatniego wywolania</param>
	    public UpdateEventArgs(TimeSpan t) { Elapsed = t; }

	    /// <summary>
		/// Cas jaki uplynal od ostatniego wywolania.
		/// </summary>
		public double UpdateTime => Elapsed.TotalSeconds;

		public TimeSpan Elapsed { get; }

	}
}
