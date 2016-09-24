using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracaInzynierska.Events {

	/// <summary>
	/// Typ argumentu eventu wywoływanego podczas poruszania mapą
	/// </summary>
	public class MapMovedEventArgs : EventArgs {

		/// <summary>
		/// Konstruktor obiektu.
		/// </summary>
		/// <param name="dx">Przesunięcie wzdloz osi Ox</param>
		/// <param name="dy">Przesuniecie wzdluz osi Oy</param>
		public MapMovedEventArgs(double dx, double dy) {
			this.dx = dx;
			this.dy = dy;
		}

		/// <summary>
		/// Przesuniecie wzdluz osi Ox
		/// </summary>
		public double dx { get; private set; }

		/// <summary>
		/// Przesuniecie wzdluz osi Oy
		/// </summary>
		public double dy { get; private set; }
	}
}
