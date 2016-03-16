using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.System;
using System.Threading.Tasks;

namespace PracaInzynierska.Events {
	class UpdateEventArgs : EventArgs {

		public UpdateEventArgs(TimeSpan t) : base() {
			UpdateTime = (float)t.TotalSeconds;
			
		}

		public float UpdateTime { get; private set; }

	}
}
