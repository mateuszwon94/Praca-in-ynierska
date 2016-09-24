using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using PracaInzynierska;
using PracaInzynierska.Events;
using PracaInzynierska.Map;

namespace PracaInzynierska.Beeing.Men {
	public class Human : MenBase {
		public Human(Window window, Map.Map map) : base(window, map) { }
	}
}
