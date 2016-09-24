using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using PracaInzynierska;
using PracaInzynierska.Events;
using PracaInzynierska.Map;

namespace PracaInzynierska.Beeing {
	public abstract class MenBase : Beeing {
		public MenBase(Window window, Map.Map map) : base(window, map) { }
		
		public override void Draw(RenderTarget target, RenderStates states) {
			if ( (Texture.Position.X >= -Texture.Texture.Size.X) &&
				(Texture.Position.X <= Program.window.Size.X) &&
				(Texture.Position.Y >= -Texture.Texture.Size.Y) &&
				(Texture.Position.Y <= Program.window.Size.Y) ) {
				target.Draw(Texture);
			}
		}
	}
}
