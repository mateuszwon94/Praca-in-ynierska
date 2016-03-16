using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracaInzynierska.Events {
	class MapMovedEventArgs : EventArgs {
		public MapMovedEventArgs(float dx, float dy) {
			this.dx = dx;
			this.dy = dy;
		}

		public float dx { get; private set; }
		public float dy { get; private set; }
	}
}
