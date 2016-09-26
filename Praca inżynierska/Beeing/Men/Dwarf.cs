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
using static System.Math;

namespace PracaInzynierska.Beeing.Men {
	public class Dwarf : MenBase {
		public Dwarf(Window window, Map.Map map) : base(window, map) { }

		public override bool InsideElement(int x, int y) {
			if ( (Location.ScreenPosition.X <= x) && (x < Location.ScreenPosition.X + Size.X) && (Location.ScreenPosition.Y <= y) && (y < Location.ScreenPosition.Y + Size.Y) ) {
				Vector2f pos = new Vector2f(x, y) - Location.ScreenPosition;
				Vector2f center = Location.Center - Location.ScreenPosition;

				if ( Pow(center.X - pos.X, 2) + Pow(center.Y - pos.Y, 2) <= Pow(MapField.Size, 2) ) return true;
			}
			return false;
		}

		protected override void UpdateTime(object sender, UpdateEventArgs e) {
			base.UpdateTime(sender, e);

			if ( IsSelected ) { }
		}
	}
}
