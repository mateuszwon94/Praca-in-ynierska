using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracaInzynierska.Events.Job {
	public class JobEventArgs : EventArgs {
		public JobEventArgs(float amout) { Amount = amout; }

		public float Amount { get; private set; }
	}
}
